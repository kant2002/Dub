// -----------------------------------------------------------------------
// <copyright file="IGenericStore.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic interface for the persistence store.
    /// </summary>
    /// <typeparam name="TEntityKey">Type of key for the entity.</typeparam>
    /// <typeparam name="TEntity">Type of entity to be persisted.</typeparam>
    public interface IGenericStore<TEntityKey, TEntity> : IDisposable
        where TEntity : class
    {
        /// <summary>
        /// Gets entities in the store.
        /// </summary>
        IQueryable<TEntity> Items { get; }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">Message object to be persisted.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        Task CreateAsync(TEntity entity);

        /// <summary>
        /// Updates existing entity.
        /// </summary>
        /// <param name="entity">Message object to be updated.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Finds the entity by it's id.
        /// </summary>
        /// <param name="id">Id of the entity to get.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        Task<TEntity> FindByIdAsync(TEntityKey id);

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        Task DeleteAsync(TEntity entity);
    }
}
