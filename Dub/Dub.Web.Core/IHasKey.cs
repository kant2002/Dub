// -----------------------------------------------------------------------
// <copyright file="IHasKey.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    /// <summary>
    /// Interface for the entities which has keys.
    /// </summary>
    /// <typeparam name="TEntityKey">Type of the entity key.</typeparam>
    public interface IHasKey<TEntityKey>
    {
        /// <summary>
        /// Gets or sets id of the entity.
        /// </summary>
        TEntityKey Id { get; set; }
    }
}
