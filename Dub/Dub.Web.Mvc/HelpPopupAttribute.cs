// -----------------------------------------------------------------------
// <copyright file="HelpPopupAttribute.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides attribute which lets you specify help information string for members of entity classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class HelpPopupAttribute : Attribute
    {
        /// <summary>
        /// Localizable string for the <see cref="HelpPopupAttribute.HelpText"/> property.
        /// </summary>
        private LocalizableString helpText = new LocalizableString("HelpText");

        /// <summary>
        /// The type that contains the localized resources.
        /// </summary>
        private Type resourceType;

        /// <summary>
        /// Gets or sets a value that is used to display a help text in the UI.
        /// </summary>
        public string HelpText
        {
            get
            {
                return this.helpText.Value;
            }

            set
            {
                if (this.helpText.Value != value)
                {
                    this.helpText.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type that contains the resources for the <see cref="HelpPopupAttribute.HelpText"/> property.
        /// </summary>
        public Type ResourceType
        {
            get
            {
                return this.resourceType;
            }

            set
            {
                if (this.resourceType != value)
                {
                    this.resourceType = value;
                    this.helpText.ResourceType = value;
                }
            }
        }

        /// <summary>
        /// Returns the value of the <see cref="HelpPopupAttribute.HelpText"/> property.
        /// </summary>
        /// <returns>
        /// The localized string for the <see cref="HelpPopupAttribute.HelpText"/> property
        /// if the <see cref="HelpPopupAttribute.ResourceType"/> property has been specified and if the
        /// <see cref="HelpPopupAttribute.HelpText"/> property represents a resource key; otherwise, 
        /// the non-localized value of the <see cref="HelpPopupAttribute.HelpText"/> property.
        /// </returns>
        public string GetHelpText()
        {
            return this.helpText.GetLocalizableValue();
        }
    }
}
