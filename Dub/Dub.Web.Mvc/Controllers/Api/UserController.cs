// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers.Api
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Dub.Web.Core;
    using Dub.Web.Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;

    /// <summary>
    /// API User controller.
    /// </summary>
    /// <typeparam name="TUser">Type of the user.</typeparam>
    /// <typeparam name="TUserManager">Type of user manager used to manage the users.</typeparam>
    /// <typeparam name="TUserFilter">Type which specify parameters to the users list.</typeparam>
    public class UserController<TUser, TUserManager, TUserFilter> : ApiControllerBase
        where TUser : DubUser, new()
        where TUserManager : UserManager<TUser, string>
        where TUserFilter : class, ICollectionFilter<TUser>, ICollectionTransform<TUser>, new()
    {
        /// <summary>
        /// User manager.
        /// </summary>
        private TUserManager userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController{TUser,TUserManager,TUserFilter}"/> class.
        /// </summary>
        public UserController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController{TUser,TUserManager,TUserFilter}"/> class.
        /// </summary>
        /// <param name="userManager">User manager for this controller.</param>
        public UserController(TUserManager userManager)
        {
            this.UserManager = userManager;
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
        /// Returns list of the users.
        /// </summary>
        /// <param name="offset">Page of the data to return.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="displayParameters">Model for the applying filtering parameters to the list.</param>
        /// <returns>Result which returns the list of the users which match the requested criteria.</returns>
        [HttpGet]
        [Route("users")]
        public IHttpActionResult Get(int offset, int pageSize, TUserFilter displayParameters)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            displayParameters = displayParameters ?? new TUserFilter();
            var sourceData = this.GetUsers();
            var preparedData = this.Filter(sourceData, displayParameters, null, true, offset, pageSize);
            var transformedData = displayParameters.Transform(preparedData);
            return this.Ok(transformedData.ToArray());
        }

        /// <summary>
        /// Returns single user.
        /// </summary>
        /// <param name="id">Id of the user to return.</param>
        /// <returns>Result which returns the list of the users which match the requested criteria.</returns>
        [HttpGet]
        [Route("users/{id}")]
        public async Task<IHttpActionResult> GetById(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            var user = await this.UserManager.FindByIdAsync(id);
            var transformedData = this.Transform(user);
            return this.Ok(transformedData);
        }

        /// <summary>
        /// Convert single user to the DTO.
        /// </summary>
        /// <param name="user">User which has to be converted to DTO.</param>
        /// <returns>Converted to DTO user.</returns>
        protected virtual object Transform(TUser user)
        {
            return user;
        }

        /// <summary>
        /// Gets the initial sequence of the users.
        /// </summary>
        /// <returns>Sequence of users to display in the list.</returns>
        protected virtual IQueryable<TUser> GetUsers()
        {
            return this.UserManager.Users;
        }
    }
}
