// -----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
#if !NETCORE
    using System.Net.Mail;
#endif
    using System.Security.Claims;
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web;
    using System.Web.Mvc;
#endif
    using Dub.Web.Core;
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.Account;
    using Dub.Web.Mvc.Properties;
#if NETCORE
    using Microsoft.AspNet.Authentication;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc.Rendering;
#endif
    using Microsoft.AspNet.Identity;
#if !NETCORE
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
#endif
#if NETCORE
    using Microsoft.AspNet.Mvc;
#endif

    /// <summary>
    /// Account controller.
    /// </summary>
    /// <typeparam name="TUser">Type of the user.</typeparam>
    /// <typeparam name="TSignInManager">Type of sign in manager used.</typeparam>
    /// <typeparam name="TUserManager">Type of user manager used to manage the users.</typeparam>
    /// <typeparam name="TRegistrationModel">Type of model used for registration.</typeparam>
    [Authorize]
    public class AccountController<TUser, TSignInManager, TUserManager, TRegistrationModel> : Controller
        where TUser : DubUser, new()
#if NETCORE
        where TSignInManager : SignInManager<TUser>
        where TUserManager : UserManager<TUser>
#else
        where TSignInManager : SignInManager<TUser, string>
        where TUserManager : UserManager<TUser, string>
#endif
        where TRegistrationModel : RegisterViewModel
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
        /// Initializes a new instance of the <see cref="AccountController{TUser,TSignInManager,TUserManager,TRegistrationModel}"/> class.
        /// </summary>
        public AccountController()
        {
        }
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController{TUser,TSignInManager,TUserManager,TRegistrationModel}"/> class.
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
                return this.signInManager ?? HttpContext.GetOwinContext().Get<TSignInManager>();
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
                return this.userManager ?? HttpContext.GetOwinContext().GetUserManager<TUserManager>();
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
                return HttpContext.GetOwinContext().Authentication;
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
        /// Shows login page for entering login information.
        /// </summary>
        /// <param name="returnUrl">Url to which user should return after login.</param>
        /// <returns>Action result.</returns>
        [AllowAnonymous]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "This design imposed by the Mvc")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        /// <summary>
        /// Performs login based on entered information from user.
        /// </summary>
        /// <param name="model">Login parameters.</param>
        /// <param name="returnUrl">Url to which user should return after login.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This design imposed by the Mvc")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await this.SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
#if !NETCORE
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, Resources.InvalidLoginAttempt);
                    return this.View(model);
            }
#else
            if (result.Succeeded)
            {
                return this.RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return this.View("Lockout");
            }

            if (result.RequiresTwoFactor)
            {
                return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }

            this.ModelState.AddModelError(string.Empty, Resources.InvalidLoginAttempt);
            return this.View(model);
#endif
        }

        /// <summary>
        /// Shows screen for verification confirmation code in two-factor authorization scenarios.
        /// </summary>
        /// <param name="provider">Provider for which verification happens.</param>
        /// <param name="returnUrl">Url to which user should return after verification.</param>
        /// <param name="rememberMe">True if authenticated user should be persisted for future visits; false otherwise.</param>
        /// <returns>Action result.</returns>
        [AllowAnonymous]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This design imposed by the Mvc")]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
#if !NETCORE
            // Require that the user has already logged in via username/password or external login
            if (!await this.SignInManager.HasBeenVerifiedAsync())
            {
                return this.View("Error");
            }
#else
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Remove before production
            // ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user, provider);
#endif
            return this.View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// Performs verification of security code generated during two-factor authentication.
        /// </summary>
        /// <param name="model">Data for verify security code.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await this.SignInManager.TwoFactorSignInAsync(model.Provider, model.Code,  model.RememberMe, model.RememberClient);
#if !NETCORE
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, Resources.InvalidVerificationCode);
                    return this.View(model);
            }
#else
            if (result.Succeeded)
            {
                return this.RedirectToLocal(model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                return this.View("Lockout");
            }

            this.ModelState.AddModelError(string.Empty, Resources.InvalidVerificationCode);
            return this.View(model);
#endif
        }

        /// <summary>
        /// Show page for entering registration information.
        /// </summary>
        /// <returns>Action result.</returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return this.View();
        }

        /// <summary>
        /// Performs registration of the user based on entered information.
        /// </summary>
        /// <param name="model">Information about user to register.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(TRegistrationModel model)
        {
            // var model = this.CreateModel();
            // this.TryUpdateModel(model);
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // If we got this far, something failed, redisplay form
            var user = this.CreateUserFromRegistrationModel(model);
            var result = await this.UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                this.AddErrors(result);
                return this.View(model);
            }

#if !NETCORE
            var userIdentity = user.Id;
            await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
#else
            var userIdentity = user;
            await this.SignInManager.SignInAsync(user, false, null);
#endif

            // Send an email with confirmation link
            string code = await this.UserManager.GenerateEmailConfirmationTokenAsync(userIdentity);
#if !NETCORE || SUPPORT_SMTP
            try
            {
                await this.SendRegistrationEmail(user, code);
            }
            catch (SmtpException ex)
            {
                var exceptionToPublish = new System.InvalidOperationException(
                    string.Format("The sending email failed for the user {0}", user.Email), 
                    ex);
                ExceptionHelper.PublishException("system", exceptionToPublish);
            }
#endif

            return this.RedirectToAction("Index", "Home");
        }

#if !NETCORE
        /// <summary>
        /// Confirm the user's email with confirmation token
        /// </summary>
        /// <param name="userId">User identifier to confirm.</param>
        /// <param name="code">Confirmation code.</param>
        /// <returns>Action result.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.View("Error");
            }

            var result = await this.UserManager.ConfirmEmailAsync(userId, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
#else
        /// <summary>
        /// Confirm the user's email with confirmation token
        /// </summary>
        /// <param name="userId">User identifier to confirm.</param>
        /// <param name="code">Confirmation code.</param>
        /// <returns>Action result.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.View("Error");
            }

            var user = await this.UserManager.FindByIdAsync(userId);
            var result = await this.UserManager.ConfirmEmailAsync(user, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
#endif

        /// <summary>
        /// Show page for the start forgot password procedure.
        /// </summary>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        /// <summary>
        /// Initiates the forgot password procedure.
        /// </summary>
        /// <param name="model">Information for the forgot password.</param>
        /// <returns>Result of the action.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.UserManager.FindByNameAsync(model.Email);
#if !NETCORE
                var userIdentity = user.Id;
#else
                var userIdentity = user;
#endif
                if (user == null || !(await this.UserManager.IsEmailConfirmedAsync(userIdentity)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await this.UserManager.GeneratePasswordResetTokenAsync(userIdentity);
                await this.SendForgotPasswordEmail(user, code);
                return this.RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        /// <summary>
        /// Show page which displays that forgot password procedure finished success.
        /// </summary>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// Show page for resetting password.
        /// </summary>
        /// <param name="userId">Id of the user which request password reset.</param>
        /// <param name="code">Security code which allows reset password.</param>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string userId, string code)
        {
            if (code == null || userId == null)
            {
                return this.View("Error");
            }

            var user = await this.UserManager.FindByIdAsync(userId);
            var model = new ResetPasswordViewModel();
            model.Code = code;
            model.Email = user.Email;
            return this.View(model);
        }

        /// <summary>
        /// Performs reset password
        /// </summary>
        /// <param name="model">Model with information about password reset.</param>
        /// <returns>Result of the action.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

#if !NETCORE
            var userIdentity = user.Id;
#else
            var userIdentity = user;
#endif
            var result = await this.UserManager.ResetPasswordAsync(userIdentity, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            this.AddErrors(result);
            return this.View();
        }

        /// <summary>
        /// Show page that password reset procedure finished successfully.
        /// </summary>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// Show page for performing external login.
        /// </summary>
        /// <param name="provider">Authentication provider to use for login.</param>
        /// <param name="returnUrl">Url to which user should be returned after authorization.</param>
        /// <returns>Result of the action.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This design imposed by the Mvc")]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            var redirectUrl = this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
#if !NETCORE
            return new ChallengeResult(provider, redirectUrl);
#else
            var properties = this.SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
#endif
        }

        /// <summary>
        /// Send security code in two-factor authentication.
        /// </summary>
        /// <param name="returnUrl">Url to which user should be returned after authorization.</param>
        /// <param name="rememberMe">True to persist user for next visits; false otherwise.</param>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "This design imposed by the Mvc")]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
#if !NETCORE
            var userId = await this.SignInManager.GetVerifiedUserIdAsync();
#else
            var userId = await this.SignInManager.GetTwoFactorAuthenticationUserAsync();
#endif
            if (userId == null)
            {
                return this.View("Error");
            }

            var userFactors = await this.UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return this.View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// Sends code in two-factor authorization.
        /// </summary>
        /// <param name="model">Model with information about sending security code.</param>
        /// <returns>Result of the action.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

#if !NETCORE
            // Generate the token and send it
            if (!await this.SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return this.View("Error");
            }
#else
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await UserManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
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
#endif

            return this.RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        /// <summary>
        /// Callback for external providers.
        /// </summary>
        /// <param name="returnUrl">Return url to which user should be redirected.</param>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "This design imposed by the Mvc")]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
#if !NETCORE
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await this.SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    this.ViewBag.ReturnUrl = returnUrl;
                    this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
#else
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return this.RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await this.SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                isPersistent: false);
            if (result.Succeeded)
            {
                return this.RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return this.View("Lockout");
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                this.ViewBag.ReturnUrl = returnUrl;
                this.ViewBag.LoginProvider = info.LoginProvider;
                // REVIEW: handle case where email not in claims?
                var email = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
#endif
        }

        /// <summary>
        /// Confirm external login.
        /// </summary>
        /// <param name="model">Model for confirming authorization using external login.</param>
        /// <param name="returnUrl">Return url to which return user.</param>
        /// <returns>Result of the action.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This design imposed by the Mvc")]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Manage");
            }

            if (!this.ModelState.IsValid)
            {
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View(model);
            }

            // Get the information about the user from the external login provider
#if !NETCORE
            var info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
#else
            var info = await this.SignInManager.GetExternalLoginInfoAsync();
#endif
            if (info == null)
            {
                return this.View("ExternalLoginFailure");
            }

            var user = new TUser { UserName = model.Email, Email = model.Email };
            var result = await this.UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                this.AddErrors(result);
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View(model);
            }

#if !NETCORE
            result = await this.UserManager.AddLoginAsync(user.Id, info.Login);
#else
            result = await this.UserManager.AddLoginAsync(user, info);
#endif
            if (!result.Succeeded)
            {
                this.AddErrors(result);
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View(model);
            }

#if !NETCORE
            await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
#else
            await this.SignInManager.SignInAsync(user, isPersistent: false);
#endif
            return this.RedirectToLocal(returnUrl);
        }

        /// <summary>
        /// Log-off user and terminate his authenticated session.
        /// </summary>
        /// <returns>Result of the action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
#if !NETCORE
        public ActionResult LogOff()
#else
        public async Task<ActionResult> LogOff()
#endif
        {
#if !NETCORE
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
#else
            await this.SignInManager.SignOutAsync();
#endif
            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Shows failure to authenticate using external login.
        /// </summary>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        /// <summary>
        /// Shows message after registration success.
        /// </summary>
        /// <returns>Result of the action.</returns>
        [AllowAnonymous]
        public ActionResult RegistrationSuccess()
        {
            return this.View();
        }

        /// <summary>
        /// Creates a model for registration information.
        /// </summary>
        /// <returns>A model with registration information.</returns>
        protected RegisterViewModel CreateModel()
        {
            return new RegisterViewModel();
        }

        /// <summary>
        /// Sends forgot password registration email.
        /// </summary>
        /// <param name="user">User which request forgot password email.</param>
        /// <param name="code">Code for forgot password registration.</param>
        /// <returns>Asynchronously send forgot password email.</returns>
        protected virtual async Task SendForgotPasswordEmail(TUser user, string code)
        {
#if !NETCORE
            var callbackUrl = this.Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await this.UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
#else
            var callbackUrl = this.Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);
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
#if !NETCORE
            var callbackUrl = this.Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await this.UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
#else
            var callbackUrl = this.Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);
            await MessageServices.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
#endif
        }

        /// <summary>
        /// Create user from registration model.
        /// </summary>
        /// <param name="model">Model for the registration model.</param>
        /// <returns>User corresponding to the model.</returns>
        protected virtual TUser CreateUserFromRegistrationModel(TRegistrationModel model)
        {
            var user = new TUser { UserName = model.Email, Email = model.Email };
            return user;
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if !NETCORE
                if (this.userManager != null)
                {
                    this.userManager.Dispose();
                    this.userManager = null;
                }

                if (this.signInManager != null)
                {
                    this.signInManager.Dispose();
                    this.signInManager = null;
                }
#endif
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds errors to model state from authorization results.
        /// </summary>
        /// <param name="result">Authorization result which errors should be added to model state.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
#if !NETCORE
                this.ModelState.AddModelError(string.Empty, error);
#else
                this.ModelState.AddModelError(string.Empty, error.Description);
#endif
            }
        }

        /// <summary>
        /// Redirect to local url if possible.
        /// </summary>
        /// <param name="returnUrl">Return url to which user should be redirected.</param>
        /// <returns>Result of the action.</returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home");
        }
    }
}