// -----------------------------------------------------------------------
// <copyright file="CreateUserViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.User
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// Model for the user creation.
    /// </summary>
    public class CreateUserViewModel
    {
        /// <summary>
        /// Gets or sets first name of the user.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [Display(Name = "FieldFirstName", ResourceType = typeof(Resources))]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of the user.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [Display(Name = "FieldLastName", ResourceType = typeof(Resources))]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets patronymic name of the user.
        /// </summary>
        [Display(Name = "FieldPatronymicName", ResourceType = typeof(Resources))]
        public string PatronymicName { get; set; }

        /// <summary>
        /// Gets or sets email user.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationMessageRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
        [Display(Name = "FieldEmail", ResourceType = typeof(Resources))]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets name where user is located.
        /// </summary>
        [Display(Name = "FieldCity", ResourceType = typeof(Resources))]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets address of the user.
        /// </summary>
        [Display(Name = "FieldAddress", ResourceType = typeof(Resources))]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets contact phone for the user.
        /// </summary>
        [Display(Name = "FieldPhone", ResourceType = typeof(Resources))]
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets roles for the user.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Property is bound by MVC")]
        [Display(Name = "FieldRoles", ResourceType = typeof(Resources))]
        public string[] Roles { get; set; }

        /// <summary>
        /// Gets or sets id of the client to which this user belongs.
        /// </summary>
        [Display(Name = "FieldClient", ResourceType = typeof(Resources))]
        [UIHint("Client")]
        public int? ClientId { get; set; }
    }
}
