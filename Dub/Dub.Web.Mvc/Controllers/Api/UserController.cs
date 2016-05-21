// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers.Api
{
    using System.Linq;
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web.Http;
#endif
    using Dub.Web.Core;
    using Dub.Web.Identity;
#if NETCORE
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
#else
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using IActionResult = System.Web.Http.IHttpActionResult;
#endif
    using Newtonsoft.Json;

    /// <summary>
    /// API User controller.
    /// </summary>
    /// <typeparam name="TUser">Type of the user.</typeparam>
    /// <typeparam name="TUserManager">Type of user manager used to manage the users.</typeparam>
    /// <typeparam name="TUserFilter">Type which specify parameters to the users list.</typeparam>
#if NETCORE
    public class UserController<TUser, TUserManager> : ApiControllerBase
#else
    public class UserController<TUser, TUserManager, TUserFilter> : ApiControllerBase
#endif
        where TUser : DubUser, new()
#if NETCORE
        where TUserManager : UserManager<TUser>
#else
        where TUserManager : UserManager<TUser, string>
        where TUserFilter : class, ICollectionFilter<TUser>, ICollectionTransform<TUser>, new()
#endif
    {
#if !NETCORE
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

#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="UserController{TUser,TUserManager,TUserFilter}"/> class.
        /// </summary>
        /// <param name="userManager">User manager for this controller.</param>
        public UserController(TUserManager userManager)
        {
            this.UserManager = userManager;
        }

#if !NETCORE
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
#else
        /// <summary>
        /// Gets user manager.
        /// </summary>
        public TUserManager UserManager { get; set; }
#endif
#if !NETCORE
        /// <summary>
        /// Returns list of the users.
        /// </summary>
        /// <param name="offset">Page of the data to return.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="parameters">JSON formatted model for the applying filtering parameters to the list.</param>
        /// <returns>Result which returns the list of the users which match the requested criteria.</returns>
        [HttpGet]
        [Route("users")]
        public IActionResult Get(int offset, int pageSize, string parameters)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(ApiStatusCode.InvalidArguments);
            }

            var displayParameters = string.IsNullOrEmpty(parameters) 
                ? new TUserFilter()
                : JsonConvert.DeserializeObject<TUserFilter>(parameters);
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
        public async Task<IActionResult> GetById(string id)
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
#endif
    }
}
