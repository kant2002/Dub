// -----------------------------------------------------------------------
// <copyright file="LoginModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// View model for the login view.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginModel"/> class.
        /// </summary>
        public LoginModel()
        {
        }

        /// <summary>
        /// Gets or sets email of the user.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [Display(Name = "FieldEmail", ResourceType = typeof(Resources))]
        [EmailAddress(ErrorMessageResourceName = "ValidationMessageEmailAddress", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets password of the user.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [DataType(DataType.Password)]
        [Display(Name = "FieldPassword", ResourceType = typeof(Resources))]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user authorization should be persisted.
        /// </summary>
        [Display(Name = "FieldRememberMe", ResourceType = typeof(Resources))]
        public bool RememberMe { get; set; }
    }
}
