// -----------------------------------------------------------------------
// <copyright file="SidebarItem.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Side bar item.
    /// </summary>
    public class SidebarItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SidebarItem"/> class.
        /// </summary>
        public SidebarItem()
        {
            this.Items = new List<SidebarItem>();
        }

        /// <summary>
        /// Gets or sets generic id for the sidebar item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets icon for the sidebar item.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets title for the sidebar.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this element is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this element is opened.
        /// </summary>
        public bool IsOpen { get; set; }

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
        /// Gets list of child items
        /// </summary>
        public List<SidebarItem> Items { get; private set; }
    }
}
