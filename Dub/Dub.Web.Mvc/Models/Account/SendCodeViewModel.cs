// -----------------------------------------------------------------------
// <copyright file="SendCodeViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Account
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    /// <summary>
    /// View model for the sending security code for two-factor authorization.
    /// </summary>
    public class SendCodeViewModel
    {
        /// <summary>
        /// Gets or sets selected login provider.
        /// </summary>
        public string SelectedProvider { get; set; }

        /// <summary>
        /// Gets or sets list of available login providers.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property is used by MVC")]
        public ICollection<SelectListItem> Providers { get; set; }
        
        /// <summary>
        /// Gets or sets a url to which return after authentication.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The design imposed by MVC")]
        public string ReturnUrl { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether need to persist authenticated user.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
