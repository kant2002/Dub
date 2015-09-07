// -----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers.Api
{
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web.Http;
#else
    using System.Linq;
#endif
    using Dub.Web.Core;
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.Account;
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
    /// API Account controller.
    /// </summary>
    /// <typeparam name="TUser">Type of the user.</typeparam>
    /// <typeparam name="TSignInManager">Type of sign in manager used.</typeparam>
    /// <typeparam name="TUserManager">Type of user manager used to manage the users.</typeparam>
    public class AccountController<TUser, TSignInManager, TUserManager> : ApiControllerBase
        where TUser : DubUser, new()
#if NETCORE
        where TSignInManager : SignInManager<TUser>
        where TUserManager : UserManager<TUser>
#else
        where TSignInManager : SignInManager<TUser, string>
        where TUserManager : UserManager<TUser, string>
#endif
    {
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
        /// Initializes a new instance of the <see cref="AccountController{TUser,TSignInManager,TUserManager}"/> class.
        /// </summary>
        public AccountController()
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController{TUser,TSignInManager,TUserManager}"/> class.
        /// </summary>
        /// <param name="userManager">User manager for this controller.</param>
        /// <param name="signInManager">Sign-in manager for this controller.</param>
        public AccountController(TUserManager userManager, TSignInManager signInManager)
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
        /// Performs registration of the API.
        /// </summary>
        /// <param name="model">Model for the performing login.</param>
        /// <returns>HTTP OK in case of success; </returns>
        [HttpPost]
        [Route("account/login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            var result = await this.SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
#if !NETCORE
            switch (result)
            {
                case SignInStatus.Success:
                    return await this.OnSuccessLogin(model.Email);
                case SignInStatus.LockedOut:
                    return this.StatusCode(ApiStatusCode.AccountLockedOut);
                case SignInStatus.RequiresVerification:
                    return this.StatusCode(ApiStatusCode.AccountRequiresVerification);
                case SignInStatus.Failure:
                default:
                    return this.StatusCode(ApiStatusCode.AuthorizationFailure);
            }
#else
            if (result.Succeeded)
            {
                return await this.OnSuccessLogin(model.Email);
            }

            if (result.IsLockedOut)
            {
                return this.StatusCode(ApiStatusCode.AccountLockedOut);
            }

            if (result.IsNotAllowed)
            {
                return this.StatusCode(ApiStatusCode.AccountRequiresVerification);
            }

            return this.StatusCode(ApiStatusCode.AuthorizationFailure);
#endif
        }

        /// <summary>
        /// Logout from the service.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpPost]
        [Route("account/logout")]
#if !NETCORE
        public IActionResult LogOff()
#else
        public async Task<IActionResult> LogOff()
#endif
        {
#if !NETCORE
            this.SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
#else
            await this.SignInManager.SignOutAsync();
#endif
            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Performs verification of security code generated during two-factor authentication.
        /// </summary>
        /// <param name="model">Data for verify security code.</param>
        /// <returns>Action result.</returns>
        public async Task<IActionResult> VerifyCode([FromBody]VerifyCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await this.SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberClient);
#if !NETCORE
            switch (result)
            {
                case SignInStatus.Success:
                    return this.StatusCode(ApiStatusCode.Ok);
                case SignInStatus.LockedOut:
                    return this.StatusCode(ApiStatusCode.AccountLockedOut);
                case SignInStatus.RequiresVerification:
                    return this.StatusCode(ApiStatusCode.AccountRequiresVerification);
                case SignInStatus.Failure:
                default:
                    return this.StatusCode(ApiStatusCode.AuthorizationFailure);
            }
#else
            if (result.Succeeded)
            {
                return this.StatusCode(ApiStatusCode.Ok);
            }

            if (result.IsLockedOut)
            {
                return this.StatusCode(ApiStatusCode.AccountLockedOut);
            }

            if (result.IsLockedOut)
            {
                return this.StatusCode(ApiStatusCode.AccountRequiresVerification);
            }

            return this.StatusCode(ApiStatusCode.AuthorizationFailure);
#endif
        }

        /// <summary>
        /// Performs registration of the user based on entered information.
        /// </summary>
        /// <param name="model">Information about user to register.</param>
        /// <returns>Action result.</returns>
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            var user = new TUser { UserName = model.Email, Email = model.Email };
            var result = await this.UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
#if !NETCORE
                return this.ErrorCode(ApiStatusCode.RegistrationFailed, result.Errors);
#else
                return this.ErrorCode(ApiStatusCode.RegistrationFailed, result.Errors.Select(_ => _.Description));
#endif
            }

#if !NETCORE
            await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            var userIdentity = user.Id;
#else
            await this.SignInManager.SignInAsync(user, false, null);
            var userIdentity = user;
#endif

            // Send an email with confirmation link
            string code = await this.UserManager.GenerateEmailConfirmationTokenAsync(userIdentity);
            await this.SendRegistrationEmail(user, code);
            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Initiates the forgot password procedure.
        /// </summary>
        /// <param name="model">Information for the forgot password.</param>
        /// <returns>Result of the action.</returns>
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            var user = await this.UserManager.FindByNameAsync(model.Email);
#if !NETCORE
            var userIdentity = user.Id;
#else
            var userIdentity = user;
#endif
            if (user == null || !(await this.UserManager.IsEmailConfirmedAsync(userIdentity)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return this.StatusCode(ApiStatusCode.Ok);
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await this.UserManager.GeneratePasswordResetTokenAsync(userIdentity);
            await this.SendForgotPasswordEmail(user, code);
            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Performs reset password
        /// </summary>
        /// <param name="model">Model with information about password reset.</param>
        /// <returns>Result of the action.</returns>
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            var user = await this.UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.StatusCode(ApiStatusCode.Ok);
            }

#if !NETCORE
            var userIdentity = user.Id;
#else
            var userIdentity = user;
#endif
            var result = await this.UserManager.ResetPasswordAsync(userIdentity, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.StatusCode(ApiStatusCode.Ok);
            }

            return this.ErrorCode(ApiStatusCode.OperationFailed, result.Errors);
        }

        /// <summary>
        /// Sends code in two-factor authorization.
        /// </summary>
        /// <param name="model">Model with information about sending security code.</param>
        /// <returns>Result of the action.</returns>
        public async Task<IActionResult> SendCode([FromBody]SendCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

#if !NETCORE
            // Generate the token and send it
            if (!await this.SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                this.StatusCode(ApiStatusCode.OperationFailed);
            }

            return this.StatusCode(ApiStatusCode.Ok);
#else
            var user = await this.SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return this.View("Error");
            }

            // Generate the token and send it
            var code = await this.UserManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return this.View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await MessageServices.SendEmailAsync(await UserManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await MessageServices.SendSmsAsync(await UserManager.GetPhoneNumberAsync(user), message);
            }

            return this.RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
#endif
        }

        /// <summary>
        /// Confirm external login.
        /// </summary>
        /// <param name="model">Model for confirming authorization using external login.</param>
        /// <returns>Result of the action.</returns>
        public async Task<IActionResult> ExternalLoginConfirmation([FromBody]ExternalLoginConfirmationViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            if (this.User.Identity.IsAuthenticated)
            {
                return this.StatusCode(ApiStatusCode.Ok);
            }

            // Get the information about the user from the external login provider
#if !NETCORE
            var info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
#else
            var info = await this.SignInManager.GetExternalLoginInfoAsync();
#endif
            if (info == null)
            {
                return this.StatusCode(ApiStatusCode.OperationFailed);
            }

            var user = new TUser { UserName = model.Email, Email = model.Email };
            var result = await this.UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return this.ErrorCode(ApiStatusCode.OperationFailed, result.Errors);
            }

#if !NETCORE
            result = await this.UserManager.AddLoginAsync(user.Id, info.Login);
#else
            result = await this.UserManager.AddLoginAsync(user, info);
#endif
            if (!result.Succeeded)
            {
                return this.ErrorCode(ApiStatusCode.OperationFailed, result.Errors);
            }

#if !NETCORE
            await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
#else
            await this.SignInManager.SignInAsync(user, false, null);
#endif

            return this.StatusCode(ApiStatusCode.Ok);
        }

        /// <summary>
        /// Sends forgot password registration email.
        /// </summary>
        /// <param name="user">User which request forgot password email.</param>
        /// <param name="code">Code for forgot password registration.</param>
        /// <returns>Asynchronously send forgot password email.</returns>
        protected virtual async Task SendForgotPasswordEmail(TUser user, string code)
        {
            var callbackUrl = this.Url.Link("Default", new { @controller = "ResetPassword", @action = "Account", userId = user.Id, code = code });
#if !NETCORE
            await this.UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
#else
            await MessageServices.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
#endif
        }

        /// <summary>
        /// Send registration email.
        /// </summary>
        /// <param name="user">Information about newly registered used.</param>
        /// <param name="code">Token which could be used for the confirming email.</param>
        /// <returns>Asynchronous task which sends registration email.</returns>
        protected virtual async Task SendRegistrationEmail(TUser user, string code)
        {
            var callbackUrl = this.Url.Link("Default", new { @controller = "ConfirmEmail", @action = "Account", userId = user.Id, code = code });
#if !NETCORE
            await this.UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
#else
            await MessageServices.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
#endif
        }

        /// <summary>
        /// Gets result which should be returned on the success login.
        /// </summary>
        /// <param name="userEmail">Email of the user.</param>
        /// <returns>Result to be returned on success login.</returns>
        protected virtual Task<IActionResult> OnSuccessLogin(string userEmail)
        {
            var result = this.StatusCode(ApiStatusCode.Ok);
            return Task.FromResult<IActionResult>(result);
        }

        /// <summary>
        /// Create user from registration model.
        /// </summary>
        /// <param name="model">Model for the registration model.</param>
        /// <returns>User corresponding to the model.</returns>
        protected virtual TUser CreateUserFromRegistrationModel(RegisterViewModel model)
        {
            var user = new TUser { UserName = model.Email, Email = model.Email };
            return user;
        }
    }
}
