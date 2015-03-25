// -----------------------------------------------------------------------
// <copyright file="GenericDetailActionProvider.cs" company="Andrey Kurdiumov">
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
    /// Generic action provider for the entity viewing.
    /// </summary>
    /// <typeparam name="T">Type of the entity for which viewing options should be provided</typeparam>
    public class GenericDetailActionProvider<T> : IActionProvider
    {
        /// <summary>
        /// Roles which allowed to view entity.
        /// </summary>
        private string[] roles;

        /// <summary>
        /// Name of the action to view.
        /// </summary>
        private string actionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDetailActionProvider{T}"/> class.
        /// </summary>
        /// <param name="actionName">Name of the action for viewing.</param>
        /// <param name="roles">Roles for which allowed viewing entity.</param>
        public GenericDetailActionProvider(string actionName, params string[] roles)
        {
            this.actionName = actionName;
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
        public virtual IEnumerable<ActionDescription> GetActions(ClaimsPrincipal principal, object item)
        {
            dynamic ditem = item;
            bool isAllowed = this.roles.Any(_ => principal.IsInRole(_));
            if (isAllowed)
            {
                yield return new ActionDescription
                {
                    Id = "Common.Detail",
                    CssClass = "info",
                    SmallCssClass = "blue",
                    Icon = "fa-eye",
                    Text = Resources.ActionDetails,
                    SortOrder = 10,
                    Action = this.actionName,
                    RouteParameters = new { id = ditem.Id },
                };
            }
        }
    }
}
