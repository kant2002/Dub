// -----------------------------------------------------------------------
// <copyright file="SidebarManager.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Manages the sidebar.
    /// </summary>
    public class SidebarManagerBase : IDisposable
    {
        /// <summary>
        /// Root sidebar item.
        /// </summary>
        private SidebarItem rootItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SidebarManagerBase"/> class.
        /// </summary>
        /// <param name="userBuilder">User for which sidebar manager will provide results.</param>
        public SidebarManagerBase(Func<ClaimsPrincipal> userBuilder)
        {
            this.User = new Lazy<ClaimsPrincipal>(userBuilder);
        }

        /// <summary>
        /// Gets user for which sidebar manager will provide information.
        /// </summary>
        public Lazy<ClaimsPrincipal> User { get; private set; }

        /// <summary>
        /// Get root item for sidebar.
        /// </summary>
        /// <returns>Root sidebar items.</returns>
        public SidebarItem GetRootSidebarItem()
        {
            if (this.rootItem != null)
            {
                return this.rootItem;
            }

            var rootItem = new SidebarItem();
            this.PopulateRootItem(rootItem);
            this.rootItem = rootItem;
            return rootItem;
        }

        /// <summary>
        /// Get sidebar item by id.
        /// </summary>
        /// <param name="id">Id of the sidebar item to find</param>
        /// <returns>Sidebar item with requested id if exists, null otherwise.</returns>
        public SidebarItem FindById(string id)
        {
            var root = this.GetRootSidebarItem();
            return FindById(id, root);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Populates the root item.
        /// </summary>
        /// <param name="rootItem">Root item to populate with child items.</param>
        protected virtual void PopulateRootItem(SidebarItem rootItem)
        {
        }

        /// <summary>
        /// Find item with given id within 
        /// </summary>
        /// <param name="id">Id of the item too look for.</param>
        /// <param name="parent">Parent item where to look item with given id.</param>
        /// <returns>Sidebar item with requested id if exists, null otherwise.</returns>
        private static SidebarItem FindById(string id, SidebarItem parent)
        {
            foreach (var item in parent.Items)
            {
                if (item.Id == id)
                {
                    return item;
                }

                var insideNested = FindById(id, item);
                if (insideNested != null)
                {
                    return insideNested;
                }
            }

            return null;
        }
    }
}
