// -----------------------------------------------------------------------
// <copyright file="DubUserManager.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
#if NETCORE
    using Microsoft.AspNet.Hosting;
#endif
    using Microsoft.AspNet.Identity;
#if NETCORE
    using Microsoft.Framework.OptionsModel;
    using Microsoft.Framework.Logging;
    using Microsoft.AspNet.Http;
#endif

    /// <summary>
    /// User manager for this application.
    /// </summary>
    /// <typeparam name="TUser">Actual type of the user.</typeparam>
    public class DubUserManager<TUser> : UserManager<TUser> 
        where TUser : DubUser, new()
    {
#if !NETCORE
        /// <summary>
        /// Initializes a new instance of the <see cref="DubUserManager{TUser}"/> class.
        /// </summary>
        /// <param name="store">Store which uses for the persisting.</param>
        public DubUserManager(IUserStore<TUser> store)
            : base(store)
        {
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="DubUserManager{TUser}"/> class.
        /// </summary>
        /// <param name="store">Store which uses for the persisting.</param>
        /// <param name="optionsAccessor">The accessor used to access the <see cref="IdentityOptions"/>.</param>
        /// <param name="passwordHasher">The password hashing implementation to use when saving passwords.</param>
        /// <param name="userValidators">A collection of <see cref="IUserValidator{TUser}"/> to validate users against.</param>
        /// <param name="passwordValidators">A collection of <see cref="IPasswordValidator{TUser}"/> to validate users against.</param>
        /// <param name="keyNormalizer">The <see cref="ILookupNormalizer"/> to use when generating index keys for users.</param>
        /// <param name="errors">The <see cref="IdentityErrorDescriber"/> used to provider error messages.</param>
        /// <param name="tokenProviders">A collection of <see cref="IUserTokenProvider{TUser}"/> to generate two-factor auth tokens.</param>
        /// <param name="logger">The logger used to log messages, warnings and errors.</param>
        /// <param name="contextAccessor">The accessor used to access the <see cref="HttpContext"/>.</param>
        public DubUserManager(
            IUserStore<TUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<TUser> passwordHasher, 
            IEnumerable<IUserValidator<TUser>> userValidators, 
            IEnumerable<IPasswordValidator<TUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger, 
            IHttpContextAccessor contextAccessor)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, contextAccessor)
        {
        }
#endif

        /// <summary>
        /// Event which fired when user is created.
        /// </summary>
        public event Action<TUser> Created;

        /// <summary>
        /// Create a user with no password
        /// </summary>
        /// <param name="user">User which should be created.</param>
        /// <returns>Task which returns information about user creation.</returns>
        public override async Task<IdentityResult> CreateAsync(TUser user)
        {
            user.Created = DateTime.UtcNow;
            user.Modified = DateTime.UtcNow;
            var result = await base.CreateAsync(user);
            var handler = this.Created;
            if (handler != null)
            {
                handler.Invoke(user);
            }

            return result;
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">User which information should be updated.</param>
        /// <returns>Task which perform updating of the user.</returns>
        public override Task<IdentityResult> UpdateAsync(TUser user)
        {
            user.Modified = DateTime.UtcNow;
            return base.UpdateAsync(user);
        }

        /// <summary>
        /// Returns accessible for given principal users.
        /// </summary>
        /// <param name="principal">Principal for which get list of accessible users.</param>
        /// <returns>IQueryable of users accessible for given principal.</returns>
        public IQueryable<TUser> GetAccessibleUsers(ClaimsPrincipal principal)
        {
            return this.FilterUsers(this.Users, principal);
        }

        /// <summary>
        /// Gets the roles which could be managed by given principal.
        /// </summary>
        /// <param name="user">User for which return roles which he could managed.</param>
        /// <returns>List of roles which could be managed by the user.</returns>
        public virtual string[] GetManagedRoles(System.Security.Principal.IPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.IsInRole(RoleNames.Administrator))
            {
                return new[] 
                {
                    RoleNames.Administrator,
                    RoleNames.ClientAdministrator,
                };
            }
            else if (user.IsInRole(RoleNames.ClientAdministrator))
            {
                return new[] 
                {
                    RoleNames.ClientAdministrator,
                };
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Returns client users.
        /// </summary>
        /// <param name="clientId">Id of the client.</param>
        /// <returns>IQueryable of committee members.</returns>
        public IQueryable<TUser> GetClientMembers(int clientId)
        {
            var testInstance = new TUser();
            if (!(testInstance is DubUserWithClient))
            {
                throw new NotSupportedException(string.Format("User class does not inherited from {0}.", typeof(DubUserWithClient).Name));
            }

            return this.Users.Cast<DubUserWithClient>()
                .Where(_ => _.ClientId == clientId)
                .Cast<TUser>();
        }

        /// <summary>
        /// Filter list of users based on availability.
        /// </summary>
        /// <param name="users">Sequence of users.</param>
        /// <param name="principal">Principal which is used for accessing data.</param>
        /// <returns>Filter sequence with applied security rules.</returns>
        protected virtual IQueryable<TUser> FilterUsers(IQueryable<TUser> users, System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            if (users == null)
            {
                throw new ArgumentNullException("users");
            }

            if (principal.IsInRole(RoleNames.Administrator))
            {
                return users;
            }

            if (principal.IsInRole(RoleNames.ClientAdministrator))
            {
                var clientId = principal.GetClient();
                users = users.Cast<DubUserWithClient>()
                    .Where(_ => _.ClientId == clientId)
                    .Cast<TUser>();
            }

            return Enumerable.Empty<TUser>().AsQueryable();
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.Created = null;
            }
        }
    }
}
