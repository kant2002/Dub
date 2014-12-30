// -----------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="Andrey Kurdiumov">
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
    public class LoginViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        public LoginViewModel()
        {
            this.DisplayRegisterLink = true;
            this.LabelWidth = 2;
            this.ControlWidth = 12 - this.LabelWidth;
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

        /// <summary>
        /// Gets or sets a value indicating whether registration link should be displayed.
        /// </summary>
        public bool DisplayRegisterLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether for users 
        /// should be displayed options for login using social providers.
        /// </summary>
        public bool DisplayLoginUsingSocial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether that this block should be initially visible.
        /// </summary>
        public bool InitiallyVisible { get; set; }

        /// <summary>
        /// Gets or sets width for the column.
        /// </summary>
        public int LabelWidth { get; set; }

        /// <summary>
        /// Gets or sets width of the control.
        /// </summary>
        public int ControlWidth { get; set; }
    }
}
