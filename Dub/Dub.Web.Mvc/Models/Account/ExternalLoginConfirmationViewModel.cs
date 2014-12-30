// -----------------------------------------------------------------------
// <copyright file="ExternalLoginConfirmationViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// View model for the confirmation of the external login.
    /// </summary>
    public class ExternalLoginConfirmationViewModel
    {
        /// <summary>
        /// Gets or sets email for the external login provider.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
