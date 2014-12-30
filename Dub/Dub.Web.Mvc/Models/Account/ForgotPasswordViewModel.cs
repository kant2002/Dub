// -----------------------------------------------------------------------
// <copyright file="ForgotPasswordViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// Model for the forget password procedure.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Gets or sets email on which send password reset request.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceName = "ValidationMessageEmailAddress", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether that this block should be initially visible.
        /// </summary>
        public bool InitiallyVisible { get; set; }
    }
}
