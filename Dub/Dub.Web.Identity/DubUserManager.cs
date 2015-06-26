// -----------------------------------------------------------------------
// <copyright file="DubUserManager.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;

    /// <summary>
    /// User manager for this application.
    /// </summary>
    /// <typeparam name="T">Actual type of the user.</typeparam>
    public class DubUserManager<T> : UserManager<T> where T : DubUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DubUserManager{T}"/> class.
        /// </summary>
        /// <param name="store">Store which uses for the persisting.</param>
        public DubUserManager(IUserStore<T> store)
            : base(store)
        {
        }

        /// <summary>
        /// Event which fired when user is created.
        /// </summary>
        public event Action<T> Created;

        /// <summary>
        /// Create a user with no password
        /// </summary>
        /// <param name="user">User which should be created.</param>
        /// <returns>Task which returns information about user creation.</returns>
        public override async Task<IdentityResult> CreateAsync(T user)
        {
            user.Created = DateTime.UtcNow;
            user.Modified = DateTime.UtcNow;
            return await base.CreateAsync(user);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">User which information should be updated.</param>
        /// <returns>Task which perform updating of the user.</returns>
        public override Task<IdentityResult> UpdateAsync(T user)
        {
            user.Modified = DateTime.UtcNow;
            return base.UpdateAsync(user);
        }

        /// <summary>
        /// Returns accessible for given principal users.
        /// </summary>
        /// <param name="principal">Principal for which get list of accessible users.</param>
        /// <returns>IQueryable of users accessible for given principal.</returns>
        public IQueryable<T> GetAccessibleUsers(ClaimsPrincipal principal)
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
        public IQueryable<T> GetClientMembers(int clientId)
        {
            if (!typeof(DubUserWithClient).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException(string.Format("User class does not inherited from {0}.", typeof(DubUserWithClient).Name));
            }

            return this.Users.Cast<DubUserWithClient>()
                .Where(_ => _.ClientId == clientId)
                .Cast<T>();
        }

        /// <summary>
        /// Filter list of users based on availability.
        /// </summary>
        /// <param name="users">Sequence of users.</param>
        /// <param name="principal">Principal which is used for accessing data.</param>
        /// <returns>Filter sequence with applied security rules.</returns>
        protected virtual IQueryable<T> FilterUsers(IQueryable<T> users, System.Security.Claims.ClaimsPrincipal principal)
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
                    .Cast<T>();
            }

            return Enumerable.Empty<T>().AsQueryable();
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
