// -----------------------------------------------------------------------
// <copyright file="ManageController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web;
    using System.Web.Mvc;
#endif
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.Manage;
    using Dub.Web.Mvc.Properties;
#if NETCORE
    using System.Security.Claims;
    using Microsoft.AspNet.Authorization;
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
    /// Controller for managing account.
    /// </summary>
    /// <typeparam name="TUser">Type of the user.</typeparam>
    /// <typeparam name="TSignInManager">Type of sign in manager used.</typeparam>
    /// <typeparam name="TUserManager">Type of user manager used to manage the users.</typeparam>
    [Authorize]
    public class ManageController<TUser, TSignInManager, TUserManager> : Controller
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
                return this.signInManager ?? this.HttpContext.GetOwinContext().Get<TSignInManager>();
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
                return this.userManager ?? this.HttpContext.GetOwinContext().GetUserManager<TUserManager>();
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
        /// Displays person account.
        /// </summary>
        /// <param name="message">Id of the message to display.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        public async Task<ActionResult> Index(ManageAccountMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageAccountMessageId.ChangePasswordSuccess ? Resources.PasswordChangedConfirmation
                : message == ManageAccountMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageAccountMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageAccountMessageId.Error ? "An error has occurred."
                : message == ManageAccountMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageAccountMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : string.Empty;

#if !NETCORE
            var userId = User.Identity.GetUserId();
#else
            var userId = await this.GetCurrentUserAsync();
#endif
            var model = new IndexViewModel
            {
                HasPassword = this.HasPassword(),
                PhoneNumber = await this.UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await this.UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await this.UserManager.GetLoginsAsync(userId),
#if !NETCORE
                BrowserRemembered = await this.AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
#else
                BrowserRemembered = await this.SignInManager.IsTwoFactorClientRememberedAsync(userId)
#endif
            };
            return this.View(model);
        }

#if !NETCORE
        /// <summary>
        /// Remove additional login from external authentication provider.
        /// </summary>
        /// <param name="loginProvider">Id of the external authentication provider.</param>
        /// <param name="providerKey">Provider specific key for the login.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageAccountMessageId? message;
            var result = await this.UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                message = ManageAccountMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageAccountMessageId.Error;
            }

            return this.RedirectToAction("ManageLogins", new { Message = message });
        }
#endif

        /// <summary>
        /// Show page for adding phone number for two-factor authentication.
        /// </summary>
        /// <returns>An action result.</returns>
        public ActionResult AddPhoneNumber()
        {
            return this.View();
        }

        /// <summary>
        /// Perform adding phone number for two-factor authentication.
        /// </summary>
        /// <param name="model">Information for adding phone number.</param>
        /// <returns>An asynchronous task which return action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

#if NETCORE
            var userId = await this.GetCurrentUserAsync();
#else
            var userId = User.Identity.GetUserId();
#endif
            // Generate the token and send it
            var code = await this.UserManager.GenerateChangePhoneNumberTokenAsync(userId, model.Number);
#if !NETCORE
            if (this.UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await this.UserManager.SmsService.SendAsync(message);
            }
#else
#endif

            return this.RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        /// <summary>
        /// Enable two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
#if !NETCORE
            var userIdentity = User.Identity.GetUserId();
            await this.UserManager.SetTwoFactorEnabledAsync(userIdentity, true);
            var user = await this.UserManager.FindByIdAsync(userIdentity);
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
#else
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await this.UserManager.SetTwoFactorEnabledAsync(user, true);
                await this.SignInManager.RefreshSignInAsync(user);
            }
#endif

            return this.RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// Disable two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
#if !NETCORE
            var userIdentity = User.Identity.GetUserId();
            await this.UserManager.SetTwoFactorEnabledAsync(userIdentity, false);
            var user = await this.UserManager.FindByIdAsync(userIdentity);
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
#else
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await this.UserManager.SetTwoFactorEnabledAsync(user, false);
                await this.SignInManager.RefreshSignInAsync(user);
            }
#endif

            return this.RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// Shows page for verifying phone number.
        /// </summary>
        /// <param name="phoneNumber">Phone number to verify.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
#if !NETCORE
            var userIdentity = User.Identity.GetUserId();
#else
            var userIdentity = await GetCurrentUserAsync();
#endif
            var code = await this.UserManager.GenerateChangePhoneNumberTokenAsync(userIdentity, phoneNumber);

            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null
                ? this.View("Error")
                : this.View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        /// <summary>
        /// Verifies phone number.
        /// </summary>
        /// <param name="model">Model with information required for phone verification.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

#if !NETCORE
            var userIdentity = User.Identity.GetUserId();
#else
            var userIdentity = await GetCurrentUserAsync();
#endif
            var result = await this.UserManager.ChangePhoneNumberAsync(userIdentity, model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
#if !NETCORE
                var user = await this.UserManager.FindByIdAsync(userIdentity);
#else
                var user = userIdentity;
#endif
                if (user != null)
                {
#if !NETCORE
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
#else
                    await SignInManager.RefreshSignInAsync(user);
#endif
                }

                return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.AddPhoneSuccess });
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, Resources.FailedToVerifyPhone);
            return this.View(model);
        }

        /// <summary>
        /// Removed phone number used within two-factor authentication.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        public async Task<ActionResult> RemovePhoneNumber()
        {
#if !NETCORE
            var userIdentity = User.Identity.GetUserId();
            var result = await this.UserManager.SetPhoneNumberAsync(userIdentity, null);
            if (!result.Succeeded)
            {
                return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.Error });
            }

            var user = await this.UserManager.FindByIdAsync(userIdentity);
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
#else
            var userIdentity = await this.GetCurrentUserAsync();
            if (userIdentity != null)
            {
                var result = await this.UserManager.SetPhoneNumberAsync(userIdentity, null);
                if (!result.Succeeded)
                {
                    return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.Error });
                }

                await this.SignInManager.RefreshSignInAsync(userIdentity);
            }
#endif

            return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.RemovePhoneSuccess });
        }

        /// <summary>
        /// Show page for changing password.
        /// </summary>
        /// <returns>Result of an action.</returns>
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        /// <summary>
        /// Performs password change.
        /// </summary>
        /// <param name="model">Model with information required to change password.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
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

                return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.ChangePasswordSuccess });
            }

            this.AddErrors(result);
            return this.View(model);
#else
            var user = await this.GetCurrentUserAsync();
            if (user != null)
            {
                var result = await this.UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await SignInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index", new { Message = ManageAccountMessageId.ChangePasswordSuccess });
                }

                this.AddErrors(result);
                return this.View(model);
            }

            return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.Error });
#endif
        }

        /// <summary>
        /// Shows page for settings password.
        /// </summary>
        /// <returns>Result of an action.</returns>
        public ActionResult SetPassword()
        {
            return this.View();
        }

        /// <summary>
        /// Performs setting password.
        /// </summary>
        /// <param name="model">Model with information required to set password.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If we got this far, something failed, redisplay form
                return this.View(model);
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

                return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.SetPasswordSuccess });
            }

            this.AddErrors(result);
            return this.View(model);
#else
            var user = await this.GetCurrentUserAsync();
            if (user != null)
            {
                var result = await this.UserManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await this.SignInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index", new { Message = ManageAccountMessageId.SetPasswordSuccess });
                }

                this.AddErrors(result);
                return this.View(model);
            }

            return this.RedirectToAction("Index", new { Message = ManageAccountMessageId.Error });
#endif
        }

        /// <summary>
        /// Shows page for managing logins.
        /// </summary>
        /// <param name="message">Id of the message to display.</param>
        /// <returns>Task which asynchronously display action result.</returns>
        public async Task<ActionResult> ManageLogins(ManageAccountMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageAccountMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageAccountMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageAccountMessageId.Error ? "An error has occurred."
                : "";
#if !NETCORE
            var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return this.View("Error");
            }

            var userLogins = await this.UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = this.AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
#else
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var userLogins = await UserManager.GetLoginsAsync(user);
            var otherLogins = SignInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
#endif
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return this.View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        /// <summary>
        /// Link login with specified provider.
        /// </summary>
        /// <param name="provider">Login provider to link current account to.</param>
        /// <returns>Result of an action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        /// <summary>
        /// Callback page which is called from external login provider after linking.
        /// </summary>
        /// <returns>Task which asynchronously display action result.</returns>
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return this.RedirectToAction("ManageLogins", new { Message = ManageAccountMessageId.Error });
            }

            var result = await this.UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded
                ? this.RedirectToAction("ManageLogins")
                : this.RedirectToAction("ManageLogins", new { Message = ManageAccountMessageId.Error });
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.userManager != null)
            {
                this.userManager.Dispose();
                this.userManager = null;
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

        /// <summary>
        /// Tests that current user has phone number.
        /// </summary>
        /// <returns>True of user has phone associated with it; false otherwise.</returns>
        private bool HasPhoneNumber()
        {
            var user = this.UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }

            return false;
        }

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