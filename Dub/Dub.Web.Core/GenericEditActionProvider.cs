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
        /// Title for the action create.
        /// </summary>
        private string createActionTitle;

        /// <summary>
        /// Title for the action edit.
        /// </summary>
        private string editActionTitle;

        /// <summary>
        /// Title for the action delete.
        /// </summary>
        private string deleteActionTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEditActionProvider{T}"/> class.
        /// </summary>
        /// <param name="createActionTitle">Title for the action create.</param>
        /// <param name="editActionTitle">Title for the action edit.</param>
        /// <param name="deleteActionTitle">Title for the action delete.</param>
        /// <param name="roles">Roles for which allowed editing entity.</param>
        public GenericEditActionProvider(string createActionTitle = "Create", string editActionTitle = "Edit", string deleteActionTitle = "Delete", params string[] roles)
        {
            this.createActionTitle = createActionTitle;
            this.editActionTitle = editActionTitle;
            this.deleteActionTitle = deleteActionTitle;
            this.roles = roles;
        }

        /// <summary>
        /// Checks whether entity is supported by this provider.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>True if action type is supported on given entity.</returns>
        public bool IsEntitySupported(object entity)
        {
            return entity is T;
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
            bool isAllowedEdit = isAllowed;
            if (item is IHasOwner)
            {
                var ownerProvider = (IHasOwner)item;
                var userId = principal.FindFirst(ClaimTypes.Sid).Value;
                isAllowedEdit &= ownerProvider.UserId == userId;
            }

            if (isAllowed)
            {
                yield return new ActionDescription
                {
                    Id = "Common.Create",
                    CssClass = "info",
                    SmallCssClass = "blue",
                    Icon = "fa-edit",
                    Text = this.createActionTitle,
                    SortOrder = 10,
                    Action = "Create",
                    NotItemOperation = true,
                };
            }
            
            if (isAllowedEdit)
            {
                yield return new ActionDescription
                {
                    Id = "Common.Edit",
                    CssClass = "info",
                    SmallCssClass = "blue",
                    Icon = "fa-edit",
                    Text = this.editActionTitle,
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
                    Text = this.deleteActionTitle,
                    SortOrder = 20,
                    Action = "Delete",
                    RouteParameters = new { id = ditem.Id },
                };
            }
        }
    }
}
