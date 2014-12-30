// -----------------------------------------------------------------------
// <copyright file="WellKnownClaims.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Well known claims
    /// </summary>
    public static class WellKnownClaims
    {
        /// <summary>
        /// Company claim
        /// </summary>
        public const string ClientClaim = "http://schemas.dubframework.org/claims/Client";

        /// <summary>
        /// Get company for the principal.
        /// </summary>
        /// <param name="principal">Principal for which get company id.</param>
        /// <returns>Id of the company which associated with given principal.</returns>
        public static int? GetClient(this ClaimsPrincipal principal)
        {
            var claim = WellKnownClaims.ClientClaim;
            return GetInt32(principal, claim);
        }

        /// <summary>
        /// Get integer value for the claim.
        /// </summary>
        /// <param name="principal">Principal for which get claim value.</param>
        /// <param name="claim">Name of the claim for which to get value.</param>
        /// <returns>Value of the claim if present, null otherwise.</returns>
        private static int? GetInt32(ClaimsPrincipal principal, string claim)
        {
            var companyClaim = principal.FindFirst(claim);
            if (companyClaim == null)
            {
                return null;
            }

            return Convert.ToInt32(companyClaim.Value);
        }
    }
}
