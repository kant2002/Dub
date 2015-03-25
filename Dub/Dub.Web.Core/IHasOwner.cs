// -----------------------------------------------------------------------
// <copyright file="IHasOwner.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    /// <summary>
    /// Indicates that entity has owner.
    /// </summary>
    public interface IHasOwner
    {
        /// <summary>
        /// Gets user which is own current entity.
        /// </summary>
        string UserId { get; }
    }
}
