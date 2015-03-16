// -----------------------------------------------------------------------
// <copyright file="IActionProvider.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    /// <summary>
    /// Provides action descriptions for the given entity.
    /// </summary>
    public interface IActionProvider
    {
        /// <summary>
        /// Checks whether entity type is supported by this provider.
        /// </summary>
        /// <param name="entityType">Type of the entity to check.</param>
        /// <returns>True if action type is supported.</returns>
        bool IsTypeSupported(Type entityType);

        /// <summary>
        /// Gets actions which action provider could add for given entity.
        /// </summary>
        /// <param name="principal">Principal which attempts to retrieve list of actions.</param>
        /// <param name="item">Entity for which actions could be provided.</param>
        /// <returns>Sequence of <see cref="ActionDescription"/> objects which represents actions.</returns>
        IEnumerable<ActionDescription> GetActions(ClaimsPrincipal principal, object item);
    }
}
