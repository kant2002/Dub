// -----------------------------------------------------------------------
// <copyright file="AddPhoneNumberViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Manage
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// View model for adding new phone for two-factor authorization.
    /// </summary>
    public class AddPhoneNumberViewModel
    {
        /// <summary>
        /// Gets or sets a new phone number for using with two-factor authorization.
        /// </summary>
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}
