// -----------------------------------------------------------------------
// <copyright file="ExternalLoginListViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Account
{
    /// <summary>
    /// View model for the list of external login providers.
    /// </summary>
    public class ExternalLoginListViewModel
    {
        /// <summary>
        /// Gets or sets url to which return after authorization.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
