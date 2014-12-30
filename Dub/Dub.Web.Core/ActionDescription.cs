// -----------------------------------------------------------------------
// <copyright file="ActionDescription.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    /// <summary>
    /// Description for the action.
    /// </summary>
    public class ActionDescription
    {
        /// <summary>
        /// Gets or sets generic id for the action.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets icon for the action.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets CSS class for the action.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets CSS class for the action.
        /// </summary>
        public string SmallCssClass { get; set; }

        /// <summary>
        /// Gets or sets name of the action.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets MVC action for the action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets MVC controller for the action.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets route parameters for the MVC action.
        /// </summary>
        public object RouteParameters { get; set; }

        /// <summary>
        /// Gets or sets sort order for the action in the all actions.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// this operation is not applicable to item.
        /// </summary>
        public bool NotItemOperation { get; set; }
    }
}
