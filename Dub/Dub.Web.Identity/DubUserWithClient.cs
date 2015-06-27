// -----------------------------------------------------------------------
// <copyright file="DubUserWithClient.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    using System.Security.Claims;
    
    /// <summary>
    /// Entity which represent user which has client.
    /// </summary>
    public class DubUserWithClient : DubUser
    {
        /// <summary>
        /// Gets or sets id of the client to which user belongs.
        /// </summary>
        public int? ClientId { get; set; }

        /// <summary>
        /// Gets or sets client entity to which this user belongs to.
        /// </summary>
        public virtual Client Client { get; set; }

#if !NETCORE
        /// <summary>
        /// Generates identity from this user entity.
        /// </summary>
        /// <typeparam name="T">Type of the user for which generate claims.</typeparam>
        /// <param name="manager">User manager which used for create identity</param>
        /// <returns>Task which return identity based on current user entity</returns>
        public override async System.Threading.Tasks.Task<System.Security.Claims.ClaimsIdentity> GenerateUserIdentityAsync<T>(Microsoft.AspNet.Identity.UserManager<T> manager)
        {
            var userIdentity = await base.GenerateUserIdentityAsync<T>(manager);
            if (this.ClientId.HasValue)
            {
                userIdentity.AddClaim(new Claim(WellKnownClaims.ClientClaim, this.ClientId.ToString()));
            }

            return userIdentity;
        }
#endif
    }
}
