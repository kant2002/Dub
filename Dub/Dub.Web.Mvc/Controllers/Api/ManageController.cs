// -----------------------------------------------------------------------
// <copyright file="ManageController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web.Http;
#else
    using System.Security.Claims;
#endif
    using Dub.Web.Core;
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.Manage;
    using Dub.Web.Mvc.Properties;
#if NETCORE
    using Microsoft.AspNet.Authorization;
#endif
    using Microsoft.AspNet.Identity;
#if !NETCORE
    using Microsoft.AspNet.Identity.Owin;
#endif
#if NETCORE
    using Microsoft.AspNet.Mvc;
#endif
#if !NETCORE
    using Microsoft.Owin.Security;
    using IActionResult = System.Web.Http.IHttpActionResult;
#endif

    /// <summary>
    /// Controller for managing account.
    /// </summary>
    /// <typeparam name="TUser">Type of the user.</typeparam>
    /// <typeparam name="TSignInManager">Type of sign in manager used.</typeparam>
    /// <typeparam name="TUserManager">Type of user manager used to manage the users.</typeparam>
    [Authorize]
    public class ManageController<TUser, TSignInManager, TUserManager> : ApiControllerBase
        where TUser : DubUser, new()
#if NETCORE
        where TSignInManager : SignInManager<TUser>
        where TUserManager : UserManager<TUser>
#else
        where TSignInManager : SignInManager<TUser, string>
        where TUserManager : UserManager<TUser, string>
#endif
    {
        /// <summary>
        /// Used for XSRF protection when adding external logins
        /// </summary>
        private const string XsrfKey = "XsrfId";

#if !NETCORE
        /// <summary>
        /// Sign-in manager.
        /// </summary>
        private TSignInManager signInManager;

        /// <summary>
        /// User manager.
        /// </summary>
        private TUserManager userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController{TUser,TSignInManager,TUserManager}"/> class.
        /// </summary>
        public ManageController()
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController{TUser,TSignInManager,TUserManager}"/> class.
        /// </summary>
        /// <param name="userManager">User manager for this controller.</param>
        /// <param name="signInManager">Sign-in manager for this controller.</param>
        public ManageController(TUserManager userManager, TSignInManager signInManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

#if !NETCORE
        /// <summary>
        /// Gets sign-in manager.
        /// </summary>
        public TSignInManager SignInManager
        {
            get
            {
                return this.signInManager ?? this.OwinContext.Get<TSignInManager>();
            }

            private set
            {
                this.signInManager = value;
            }
        }

        /// <summary>
        /// Gets user manager.
        /// </summary>
        public TUserManager UserManager
        {
            get
            {
                return this.userManager ?? this.OwinContext.GetUserManager<TUserManager>();
            }

            private set
            {
                this.userManager = value;
            }
        }

        /// <summary>
        /// Gets authentication manager.
        /// </summary>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return this.OwinContext.Authentication;
            }
        }
#else
        /// <summary>
        /// Gets sign-in manager.
        /// </summary>
        public TSignInManager SignInManager { get; set; }

        /// <summary>
        /// Gets user manager.
        /// </summary>
        public TUserManager UserManager { get; set; }
#endif

        /// <summary>
        /// Displays person account.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        public async Task<IActionResult> Index()
        {
#if !NETCORE
            var userId = User.Identity.GetUserId();
            var userLogins = await this.UserManager.GetLoginsAsync(userId);
            var otherLogins = this.AuthenticationManager.GetExternalAuthenticationTypes()
                .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
                .ToList();
#else
            var userId = await this.GetCurrentUserAsync();
            var userLogins = await this.UserManager.GetLoginsAsync(userId);
            var otherLogins = this.SignInManager.GetExternalAuthenticationSchemes()
                .Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider))
                .ToList();
#endif
            var model = new AccountInformationResponse
            {
                Code = ApiStatusCode.Ok,
#if !NETCORE
                HasPassword = this.HasPassword(),
#else
                HasPassword = await this.HasPassword(),
#endif
                PhoneNumber = await this.UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await this.UserManager.GetTwoFactorEnabledAsync(userId),
#if !NETCORE
                BrowserRemembered = await this.AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
#else
                BrowserRemembered = await this.SignInManager.IsTwoFactorClientRememberedAsync(userId),
#endif
                Logins = userLogins,
                OtherLogins = otherLogins,
            };
            return this.Ok(model);
        }

        /// <summary>
        /// Remove additional login from external authentication provider.
        /// </summary>
        /// <param name="loginProvider">Id of the external authentication provider.</param>
        /// <param name="providerKey">Provider specific key for the login.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

#if !NETCORE
            var result = await this.UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return this.StatusCode(ApiStatusCode.Ok);
            }
#else
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await this.UserManager.RemoveLoginAsync(user, loginProvider, providerKey);
                await this.SignInManager.SignInAsync(user, isPersistent: false);
                return this.StatusCode(ApiStatusCode.Ok);
            }
#endif

            return this.StatusCode(ApiStatusCode.RemoveLoginError);
        }

        /// <summary>
        /// Perform adding phone number for two-factor authentication.
        /// </summary>
        /// <param name="model">Information for adding phone number.</param>
        /// <returns>An asynchronous task which return action result.</returns>
        [HttpPost]
        public async Task<IActionResult> AddPhoneNumber([FromBody]AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            // Generate the token and send it
#if !NETCORE
            var code = await this.UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
#else
            var user = await this.GetCurrentUserAsync();
            var code = await this.UserManager.GenerateChangePhoneNumberTokenAsync(user, model.Number);
#endif
            var messageText = "Your security code is: " + code;
#if !NETCORE
            if (this.UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = messageText
                };
                await this.UserManager.SmsService.SendAsync(message);
            }
#else
            await MessageServices.SendSmsAsync(model.Number, messageText);
#endif
            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Enable two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
#if !NETCORE
            await this.UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
#else
            var user = await this.GetCurrentUserAsync();
            if (user != null)
            {
                await this.UserManager.SetTwoFactorEnabledAsync(user, true);
                await this.SignInManager.SignInAsync(user, isPersistent: false);
            }
#endif

            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Disable two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
#if !NETCORE
            await this.UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
#else
            var user = await this.GetCurrentUserAsync();
            if (user != null)
            {
                await this.UserManager.SetTwoFactorEnabledAsync(user, false);
                await this.SignInManager.SignInAsync(user, isPersistent: false);
            }
#endif

            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Verifies phone number.
        /// </summary>
        /// <param name="model">Model with information required for phone verification.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IActionResult> VerifyPhoneNumber([FromBody]VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

#if !NETCORE
            var result = await this.UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return this.StatusCode(ApiStatusCode.Ok);
            }

            // If we got this far, something failed, redisplay form
            return this.ErrorCode(ApiStatusCode.InvalidArguments, new string[] { Resources.FailedToVerifyPhone });
#else
            var user = await this.GetCurrentUserAsync();
            if (user != null)
            {
                var result = await this.UserManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false);
                    return this.StatusCode(ApiStatusCode.Ok);
                }

                // If we got this far, something failed, redisplay form
                return this.ErrorCode(ApiStatusCode.InvalidArguments, new string[] { Resources.FailedToVerifyPhone });
            }

            return this.StatusCode(ApiStatusCode.Ok);
#endif
        }

        /// <summary>
        /// Removed phone number used within two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IActionResult> RemovePhoneNumber()
        {
#if !NETCORE
            var result = await this.UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return this.StatusCode(ApiStatusCode.OperationFailed);
            }

            var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            return this.StatusCode(ApiStatusCode.Ok);
#else
            var user = await this.GetCurrentUserAsync();
            var result = await this.UserManager.SetPhoneNumberAsync(user, null);
            if (!result.Succeeded)
            {
                return this.StatusCode(ApiStatusCode.OperationFailed);
            }

            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false);
            }

            return this.StatusCode(ApiStatusCode.Ok);
#endif
        }

        /// <summary>
        /// Performs password change.
        /// </summary>
        /// <param name="model">Model with information required to change password.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [Route("manage/password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

#if !NETCORE
            var result = await this.UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return this.StatusCode(ApiStatusCode.Ok);
            }

            return this.ErrorCode(ApiStatusCode.OperationFailed, result.Errors);
#else
            var user = await this.GetCurrentUserAsync();
            var result = await this.UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return this.ErrorCode(ApiStatusCode.OperationFailed, result.Errors);
            }

            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false);
            }

            return this.StatusCode(ApiStatusCode.Ok);
#endif
        }

        /// <summary>
        /// Performs setting password.
        /// </summary>
        /// <param name="model">Model with information required to set password.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [Route("manage/password/add")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

#if !NETCORE
            var result = await this.UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return this.StatusCode(ApiStatusCode.Ok);
            }

            return this.ErrorCode(ApiStatusCode.OperationFailed, result.Errors);
#else
            var user = await this.GetCurrentUserAsync();
            if (user != null)
            {
                var result = await this.UserManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false);
                    return this.StatusCode(ApiStatusCode.Ok);
                }

                return this.ErrorCode(ApiStatusCode.OperationFailed, result.Errors);
            }

            return this.StatusCode(ApiStatusCode.Ok);
#endif
        }

#if !NETCORE
        /// <summary>
        /// Tests that current user has password.
        /// </summary>
        /// <returns>True of user has password; false otherwise.</returns>
        private bool HasPassword()
        {
            var user = this.UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }

            return false;
        }
#else
        /// <summary>
        /// Tests that current user has password.
        /// </summary>
        /// <returns>True of user has password; false otherwise.</returns>
        private async Task<bool> HasPassword()
        {
            var user = await this.UserManager.FindByIdAsync(this.User.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }

            return false;
        }
#endif

#if NETCORE
        /// <summary>
        /// Get current user entity.
        /// </summary>
        /// <returns>Current entity.</returns>
        private async Task<TUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(this.Context.User.GetUserId());
        }
#endif
    }
}
