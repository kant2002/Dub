// -----------------------------------------------------------------------
// <copyright file="ManageLoginsViewModel.cs" company="Andrey Kurdiumov">
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
    using AuthenticationScheme = Microsoft.Owin.Security.AuthenticationDescription;
#else
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authentication;
#if NETSTANDARD1_6
    using AuthenticationScheme = Microsoft.AspNetCore.Http.Authentication.AuthenticationDescription;
#endif
#endif

    /// <summary>
    /// View model for managing logins from different providers.
    /// </summary>
    public class ManageLoginsViewModel
    {
        /// <summary>
        /// Gets or sets list of available logins.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property is used by MVC")]
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        /// Gets or sets list of available providers.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property is used by MVC")]
        public IList<AuthenticationScheme> OtherLogins { get; set; }
    }
}
