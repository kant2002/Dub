// -----------------------------------------------------------------------
// <copyright file="DubUser.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// Represents application user.
    /// </summary>
    public class DubUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets patronymic name of the user.
        /// </summary>
        public string PatronymicName { get; set; }

        /// <summary>
        /// Gets or sets name where user is located.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets address of the user.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets contact phone for the user.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets date when user was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets date when user was modified.
        /// </summary>
        public DateTime Modified { get; set; }

#if !NETCORE
        /// <summary>
        /// Generates identity from this user entity.
        /// </summary>
        /// <typeparam name="T">Type of the user for which generate claims.</typeparam>
        /// <param name="manager">User manager which used for create identity</param>
        /// <returns>Task which return identity based on current user entity</returns>
        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync<T>(UserManager<T> manager)
            where T : DubUser
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync((T)this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            userIdentity.AddClaim(new Claim(ClaimTypes.Sid, this.Id == null ? string.Empty : this.Id.ToString()));
            userIdentity.AddClaim(new Claim(ClaimTypes.Surname, this.LastName ?? string.Empty));
            userIdentity.AddClaim(new Claim(ClaimTypes.GivenName, this.FirstName ?? string.Empty));
            
            return userIdentity;
        }
#else
        /// <summary>
        /// Add claims to identity.
        /// </summary>
        /// <param name="identity">Identity to which add user claims.</param>
        public virtual void AddClaims(ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim(ClaimTypes.Sid, this.Id == null ? string.Empty : this.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Surname, this.LastName ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, this.FirstName ?? string.Empty));
        }
#endif
    }
}
