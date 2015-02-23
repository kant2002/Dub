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
    using System.Web.Http;
    using Dub.Web.Core;
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.Manage;
    using Dub.Web.Mvc.Properties;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    /// <summary>
    /// Controller for managing account.
    /// </summary>
    /// <typeparam name="TUser">Type of the user.</typeparam>
    /// <typeparam name="TSignInManager">Type of sign in manager used.</typeparam>
    /// <typeparam name="TUserManager">Type of user manager used to manage the users.</typeparam>
    [Authorize]
    public class ManageController<TUser, TSignInManager, TUserManager> : ApiControllerBase
        where TUser : DubUser, new()
        where TSignInManager : SignInManager<TUser, string>
        where TUserManager : UserManager<TUser, string>
    {
        /// <summary>
        /// Used for XSRF protection when adding external logins
        /// </summary>
        private const string XsrfKey = "XsrfId";

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

        /// <summary>
        /// Displays person account.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        public async Task<IHttpActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var userLogins = await this.UserManager.GetLoginsAsync(userId);
            var otherLogins = this.AuthenticationManager.GetExternalAuthenticationTypes()
                .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
                .ToList();
            var model = new AccountInformationResponse
            {
                Code = ApiStatusCode.Ok,
                HasPassword = this.HasPassword(),
                PhoneNumber = await this.UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await this.UserManager.GetTwoFactorEnabledAsync(userId),
                BrowserRemembered = await this.AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
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
        public async Task<IHttpActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
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
            else
            {
                return this.StatusCode(ApiStatusCode.RemoveLoginError);
            }
        }

        /// <summary>
        /// Perform adding phone number for two-factor authentication.
        /// </summary>
        /// <param name="model">Information for adding phone number.</param>
        /// <returns>An asynchronous task which return action result.</returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            // Generate the token and send it
            var code = await this.UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (this.UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await this.UserManager.SmsService.SendAsync(message);
            }

            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Enable two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IHttpActionResult> EnableTwoFactorAuthentication()
        {
            await this.UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Disable two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IHttpActionResult> DisableTwoFactorAuthentication()
        {
            await this.UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Verifies phone number.
        /// </summary>
        /// <param name="model">Model with information required for phone verification.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IHttpActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

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
        }

        /// <summary>
        /// Removed phone number used within two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IHttpActionResult> RemovePhoneNumber()
        {
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
        }

        /// <summary>
        /// Performs password change.
        /// </summary>
        /// <param name="model">Model with information required to change password.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

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
        }

        /// <summary>
        /// Performs setting password.
        /// </summary>
        /// <param name="model">Model with information required to set password.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        public async Task<IHttpActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

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
        }

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
    }
}
