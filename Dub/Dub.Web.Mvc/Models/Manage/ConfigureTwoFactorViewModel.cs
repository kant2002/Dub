// -----------------------------------------------------------------------
// <copyright file="ConfigureTwoFactorViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Manage
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
#if !NETCORE
    using System.Web.Mvc;
#else
    using Microsoft.AspNetCore.Mvc.Rendering;
#endif

    /// <summary>
    /// View model for the two-factor authorization configuration page.
    /// </summary>
    public class ConfigureTwoFactorViewModel
    {
        /// <summary>
        /// Gets or sets external login provider selected.s
        /// </summary>
        public string SelectedProvider { get; set; }

        /// <summary>
        /// Gets or sets list of available login providers.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property is binded by MVC")]
        public ICollection<SelectListItem> Providers { get; set; }
    }
}
