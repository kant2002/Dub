// -----------------------------------------------------------------------
// <copyright file="AccountInformationResponse.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Manage
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
#if !NETCORE
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;
#else
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Http.Authentication;
#endif

    /// <summary>
    /// Response with information about player account.
    /// </summary>
    public class AccountInformationResponse : ApiStatusResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether current user has password or not.
        /// </summary>
        public bool HasPassword { get; set; }

        /// <summary>
        /// Gets or sets list of logins which is associated with the user.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property is binded by MVC")]
        public IList<UserLoginInfo> Logins { get; set; }

        /// <summary>
        /// Gets or sets list of available providers.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property is binded by MVC")]
        public IList<AuthenticationDescription> OtherLogins { get; set; }

        /// <summary>
        /// Gets or sets phone number for the current user.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether two factor authentication is supported for the current user.
        /// </summary>
        public bool TwoFactor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user is saved in the current browser.
        /// </summary>
        public bool BrowserRemembered { get; set; }
    }
}
