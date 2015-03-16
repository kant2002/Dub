// -----------------------------------------------------------------------
// <copyright file="GenericEditActionProvider.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Dub.Web.Core.Properties;

    /// <summary>
    /// Generic action provider for the entity editing.
    /// </summary>
    /// <typeparam name="T">Type of the entity for which edition options should be provided</typeparam>
    public class GenericEditActionProvider<T> : IActionProvider
    {
        /// <summary>
        /// Roles which allowed to edit entity.s
        /// </summary>
        private string[] roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEditActionProvider{T}"/> class.
        /// </summary>
        /// <param name="roles">Roles for which allowed editing entity.</param>
        public GenericEditActionProvider(params string[] roles)
        {
            this.roles = roles;
        }

        /// <summary>
        /// Checks whether entity type is supported by this provider.
        /// </summary>
        /// <param name="entityType">Type of the entity to check.</param>
        /// <returns>True if action type is supported.</returns>
        public bool IsTypeSupported(Type entityType)
        {
            return typeof(T).IsAssignableFrom(entityType);
        }

        /// <summary>
        /// Gets actions which action provider could add for given entity.
        /// </summary>
        /// <param name="principal">Principal which attempts to retrieve list of actions.</param>
        /// <param name="item">Entity for which actions could be provided.</param>
        /// <returns>Sequence of <see cref="ActionDescription"/> objects which represents actions.</returns>
        public IEnumerable<ActionDescription> GetActions(ClaimsPrincipal principal, object item)
        {
            dynamic ditem = item;
            bool isAllowed = this.roles.Any(_ => principal.IsInRole(_));
            if (isAllowed)
            {
                yield return new ActionDescription
                {
                    Id = "Common.Create",
                    CssClass = "info",
                    SmallCssClass = "blue",
                    Icon = "fa-edit",
                    Text = Resources.ActionCreate,
                    SortOrder = 10,
                    Action = "Create",
                    NotItemOperation = true,
                };
                yield return new ActionDescription
                {
                    Id = "Common.Edit",
                    CssClass = "info",
                    SmallCssClass = "blue",
                    Icon = "fa-edit",
                    Text = Resources.ActionEdit,
                    SortOrder = 10,
                    Action = "Edit",
                    RouteParameters = new { id = ditem.Id },
                };
                yield return new ActionDescription
                {
                    Id = "Common.Delete",
                    CssClass = "danger",
                    SmallCssClass = "red",
                    Icon = "fa-remove",
                    Text = Resources.ActionDelete,
                    SortOrder = 20,
                    Action = "Delete",
                    RouteParameters = new { id = ditem.Id },
                };
            }
        }
    }
}
