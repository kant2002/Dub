// -----------------------------------------------------------------------
// <copyright file="ErrorLogViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Security
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// View model for the error log entry.
    /// </summary>
    public class ErrorLogViewModel
    {
        /// <summary>
        /// Gets or sets id of the log entry.
        /// </summary>
        [Display(Name = "FieldId", ResourceType = typeof(Resources))]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user which produce event.
        /// </summary>
        [Display(Name = "FieldUserName", ResourceType = typeof(Resources))]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets date when event happens.
        /// </summary>
        [Display(Name = "FieldCreatedDate", ResourceType = typeof(Resources))]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets type of the object which was changed.
        /// </summary>
        [Display(Name = "FieldMessage", ResourceType = typeof(Resources))]
        public string ErrorMessage { get; set; }
    }
}
