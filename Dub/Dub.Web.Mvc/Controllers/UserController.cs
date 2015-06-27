// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
#if !NETCORE
    using System.Web.Mvc;
#endif
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.User;
    using Microsoft.AspNet.Identity;
#if NETCORE
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
#endif
#if !NETCORE
    using Microsoft.AspNet.Identity.Owin;
#endif

    /// <summary>
    /// Controller for managing users.
    /// </summary>
    /// <typeparam name="TUser">Type of the user which this controller will manage.</typeparam>
    /// <typeparam name="TApplicationUserManager">
    /// Type of application manager which will be used for the managing users
    /// </typeparam>
    /// <typeparam name="TCreateUserViewModel">Model for the user creation.</typeparam>
    /// <typeparam name="TEditUserViewModel">Model for the user editing.</typeparam>
    [Authorize]
    public class UserController<TUser, TApplicationUserManager, TCreateUserViewModel, TEditUserViewModel> : Controller
        where TUser : DubUser, new()
        where TApplicationUserManager : DubUserManager<TUser>
        where TCreateUserViewModel : CreateUserViewModel, new()
        where TEditUserViewModel : EditUserViewModel, new()
    {
        /// <summary>
        /// User manager.
        /// </summary>
        private TApplicationUserManager userManager;

        /// <summary>
        /// Gets user manager.
        /// </summary>
        public TApplicationUserManager UserManager
        {
            get
            {
                return this.userManager ?? HttpContext.GetOwinContext().GetUserManager<TApplicationUserManager>();
            }

            private set
            {
                this.userManager = value;
            }
        }

        /// <summary>
        /// Displays list of all users.
        /// </summary>
        /// <returns>Result of the action.</returns>
        [Authorize(Roles = RoleNames.Administrator)]
        public ActionResult Index()
        {
            var principal = (System.Security.Claims.ClaimsPrincipal)HttpContext.User;
            var users = this.UserManager.GetAccessibleUsers(principal);
            var isAdmin = principal.IsInRole(RoleNames.Administrator);
            var model = new UsersListViewModel
            {
                Users = users,
            };
            return this.View(model);
        }

        /// <summary>
        /// Displays list of the users which wait for approval of the registration.
        /// </summary>
        /// <returns>Result of the action.</returns>
        public ActionResult Pending()
        {
            var principal = (System.Security.Claims.ClaimsPrincipal)HttpContext.User;
            var notConfirmedUsers = this.UserManager.GetAccessibleUsers(principal)
                .Where(_ => !_.EmailConfirmed);
            var isAdmin = principal.IsInRole(RoleNames.Administrator);
            var model = new UsersListViewModel
            {
                Users = notConfirmedUsers,
            };
            return this.View(model);
        }

        /// <summary>
        /// Show create user page.
        /// </summary>
        /// <returns>Result of the action.</returns>
        public ActionResult Create()
        {
            var model = new CreateUserViewModel();
            return this.View(model);
        }

        /// <summary>
        /// Saves changes during creation of the user.
        /// </summary>
        /// <param name="model">Data to use when create user.</param>
        /// <returns>Task which returns result of the action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TCreateUserViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var context = HttpContext.GetOwinContext();
            var userManager = context.Get<TApplicationUserManager>();
            var claimPrincipal = (System.Security.Claims.ClaimsPrincipal)this.User;
            var user = new TUser();
            user.Email = model.Email;
            user.UserName = model.Email;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PatronymicName = model.PatronymicName;
            user.City = model.City;
            user.Address = model.Address;
            user.ContactPhone = model.ContactPhone;
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                // Add errors.
                this.AddErrors(result);
                return this.View(model);
            }

            model.Roles = this.SanitizeRoles(model.Roles ?? new string[0]);

            var currentRoles = await userManager.GetRolesAsync(user.Id);

            // Add new roles
            var rolesAdded = model.Roles.Except(currentRoles).ToArray();
            result = await userManager.AddToRolesAsync(user.Id, rolesAdded);
            if (!result.Succeeded)
            {
                // Add errors.
                this.AddErrors(result);
                return this.View(model);
            }

            // Remove roles
            var rolesRemoved = currentRoles.Except(model.Roles).ToArray();
            result = await userManager.RemoveFromRolesAsync(user.Id, rolesRemoved);
            if (!result.Succeeded)
            {
                this.AddErrors(result);
                return this.View(model);
            }

            // Send an email with confirmation link
            // string code = await this.UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            // var callbackUrl = this.Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            // await this.UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Show edit user page by id.
        /// </summary>
        /// <param name="id">Id of the user to get</param>
        /// <returns>Task which returns result of the action.</returns>
        public async Task<ActionResult> Edit(string id)
        {
            var claimPrincipal = (System.Security.Claims.ClaimsPrincipal)this.User;
            var context = HttpContext.GetOwinContext();
            var userManager = context.Get<TApplicationUserManager>();
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                // Add notification that user does not found.
                return this.RedirectToAction("Index");
            }

            var model = await this.CreateUserModel(user);
            return this.View(model);
        }

        /// <summary>
        /// Saves changes during editing of the user.
        /// </summary>
        /// <param name="model">Data to save about user.</param>
        /// <returns>Task which returns result of the action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TEditUserViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var claimPrincipal = (System.Security.Claims.ClaimsPrincipal)this.User;
            var context = HttpContext.GetOwinContext();
            var userManager = context.Get<TApplicationUserManager>();
            var user = await userManager.FindByIdAsync(model.Id);
            if (!await this.UpdateUser(model, user))
            {
                return this.View(model);
            }

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Show delete prompt for user by it's id.
        /// </summary>
        /// <param name="id">Id of the user to get</param>
        /// <returns>Task which returns result of the action.</returns>
        public async Task<ActionResult> Delete(string id)
        {
            var context = HttpContext.GetOwinContext();
            var userManager = context.Get<TApplicationUserManager>();
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                // Add notification that user does not found.
                return this.RedirectToAction("Index");
            }

            var model = await this.CreateUserModel(user);
            return this.View(model);
        }

        /// <summary>
        /// Show delete prompt for user by it's id.
        /// </summary>
        /// <param name="model">New data about the user to delete.</param>
        /// <returns>Task which returns result of the action.</returns>
        [HttpPost]
        public async Task<ActionResult> Delete(TEditUserViewModel model)
        {
            var context = HttpContext.GetOwinContext();
            var userManager = context.Get<TApplicationUserManager>();
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                // Add notification that user does not found.
                return this.RedirectToAction("Index");
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Add errors.
                this.AddErrors(result);
                return this.View(model);
            }

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Show delete prompt for user by it's id.
        /// </summary>
        /// <param name="id">Id of the user to get</param>
        /// <returns>Task which returns result of the action.</returns>
        public async Task<ActionResult> Activate(string id)
        {
            var context = HttpContext.GetOwinContext();
            var userManager = context.Get<TApplicationUserManager>();
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                // Add notification that user does not found.
                return this.RedirectToAction("Index");
            }

            user.EmailConfirmed = true;
            var result = await userManager.UpdateAsync(user);

            return this.ReturnToPreviousPage();
        }

        /// <summary>
        /// Show delete prompt for user by it's id.
        /// </summary>
        /// <param name="id">Id of the user to get</param>
        /// <returns>Task which returns result of the action.</returns>
        public async Task<ActionResult> Deactivate(string id)
        {
            var context = HttpContext.GetOwinContext();
            var userManager = context.Get<TApplicationUserManager>();
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                // Add notification that user does not found.
                return this.RedirectToAction("Index");
            }

            user.EmailConfirmed = false;
            var result = await userManager.UpdateAsync(user);

            return this.ReturnToPreviousPage();
        }

        /// <summary>
        /// Filter out roles which user could not manage.
        /// </summary>
        /// <param name="roles">Roles which user has.</param>
        /// <returns>Roles which user allowed to manage.</returns>
        protected virtual string[] SanitizeRoles(string[] roles)
        {
            string[] allowedManageRoles;
            if (this.User.IsInRole(RoleNames.Administrator))
            {
                allowedManageRoles = new[] 
                {
                    RoleNames.Administrator,
                };
            }
            else
            {
                allowedManageRoles = new string[0];
            }

            allowedManageRoles = roles.Intersect(allowedManageRoles).ToArray();
            return allowedManageRoles;
        }

        /// <summary>
        /// Create edit users model.
        /// </summary>
        /// <param name="user">User for which create edit model.</param>
        /// <returns>Edit model for the user.</returns>
        protected virtual async Task<TEditUserViewModel> CreateUserModel(TUser user)
        {
            var model = new TEditUserViewModel();
            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.PatronymicName = user.PatronymicName;
            model.City = user.City;
            model.Address = user.Address;
            model.ContactPhone = user.ContactPhone;
            var roles = await this.UserManager.GetRolesAsync(user.Id);
            model.Roles = roles.ToArray();
            return model;
        }

        /// <summary>
        /// Asynchronously update the user entity with the model information.
        /// </summary>
        /// <param name="model">Model with information to update user entity.</param>
        /// <param name="user">User entity to update with model information.</param>
        /// <returns>Task which asynchronously return status of update.</returns>
        protected virtual async Task<bool> UpdateUser(TEditUserViewModel model, TUser user)
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PatronymicName = model.PatronymicName;
            user.City = model.City;
            user.Address = model.Address;
            user.ContactPhone = model.ContactPhone;

            var result = await this.UserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // Add errors.
                this.AddErrors(result);
                return false;
            }

            model.Roles = this.SanitizeRoles(model.Roles ?? new string[0]);

            var currentRoles = await this.UserManager.GetRolesAsync(user.Id);

            // Add new roles
            var rolesAdded = model.Roles.Except(currentRoles).ToArray();
            result = await this.UserManager.AddToRolesAsync(user.Id, rolesAdded);
            if (!result.Succeeded)
            {
                // Add errors.
                this.AddErrors(result);
                return false;
            }

            // Remove roles
            var rolesRemoved = currentRoles.Except(model.Roles).ToArray();
            result = await this.UserManager.RemoveFromRolesAsync(user.Id, rolesRemoved);
            if (!result.Succeeded)
            {
                this.AddErrors(result);
                return false;
            }

            return true;
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
        /// Redirect to page from which this action was called.
        /// </summary>
        /// <returns>Result of action execution.</returns>
        private ActionResult ReturnToPreviousPage()
        {
            // Return to the same page as before.
            // This should be checked to be called from correct
            // address, since somebody from untrusted website could execute our 
            // method and returns to their website.
            // We should always return to our website, when in doubts.
            return this.Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }
    }
}