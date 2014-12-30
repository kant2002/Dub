// -----------------------------------------------------------------------
// <copyright file="ResetPasswordViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// View model for password reset procedure.
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Gets or sets email for which reset password.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceName = "ValidationMessageEmailAddress", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [Display(Name = "FieldEmail", ResourceType = typeof(Resources))]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets new value for the password.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [StringLength(100, ErrorMessageResourceName = "ValidationMessageStringLength", ErrorMessageResourceType = typeof(Resources), MinimumLength = 6, ErrorMessage = null)]
        [DataType(DataType.Password)]
        [Display(Name = "FieldPassword", ResourceType = typeof(Resources))]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets confirmation for the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "FieldConfirmPassword", ResourceType = typeof(Resources))]
        [Compare("Password", ErrorMessageResourceName = "ValidationMessageCompare", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets security token for reset password.
        /// </summary>
        public string Code { get; set; }
    }
}
