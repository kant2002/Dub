// -----------------------------------------------------------------------
// <copyright file="EntityStore.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
#if !NETCORE
    using System.Data.Entity;
#else
    using Microsoft.EntityFrameworkCore;
#endif
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic implementation of the persistence store.
    /// </summary>
    /// <typeparam name="TEntityKey">Type of key for the entity.</typeparam>
    /// <typeparam name="TEntity">Type of entity to be persisted.</typeparam>
    public class EntityStore<TEntityKey, TEntity> : IGenericStore<TEntityKey, TEntity>
        where TEntity : class
        where TEntityKey : IEquatable<TEntityKey>
    {
        /// <summary>
        /// A value indicating that object is disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Database context to use for persistence.
        /// </summary>
        private readonly DbContext context;

        /// <summary>
        /// Mapper for the id property.
        /// </summary>
        private readonly Func<TEntity, TEntityKey> idMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityStore{TEntityKey,TEntity}"/> class.
        /// </summary>
        /// <param name="context">Database context to use for persistence.</param>
        /// <param name="idMapper">Function which map id of the entity.</param>
        public EntityStore(DbContext context, Func<TEntity, TEntityKey> idMapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.idMapper = idMapper ?? throw new ArgumentNullException(nameof(idMapper));
        }

        /// <summary>
        /// Gets entities in the store.
        /// </summary>
        public IQueryable<TEntity> Items
        {
            get
            {
                this.ThrowIfDisposed();
                return this.context.Set<TEntity>();
            }
        }

        /// <summary>
        /// Gets database context.
        /// </summary>
        protected DbContext Context
        {
            get
            {
                this.ThrowIfDisposed();
                return this.context;
            }
        }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">Message object to be persisted.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        public async Task CreateAsync(TEntity entity)
        {
            this.ThrowIfDisposed();
            this.context.Set<TEntity>().Add(entity);
            await this.SaveAsync();
        }

        /// <summary>
        /// Updates existing entity.
        /// </summary>
        /// <param name="entity">Message object to be updated.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        public async Task UpdateAsync(TEntity entity)
        {
            this.ThrowIfDisposed();
            this.context.Entry(entity).State = EntityState.Modified;
            await this.SaveAsync();
        }

        /// <summary>
        /// Finds the entity by it's id.
        /// </summary>
        /// <param name="id">Id of the entity to get.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        public async Task<TEntity> FindByIdAsync(TEntityKey id)
        {
            this.ThrowIfDisposed();
            return await this.context.Set<TEntity>().FirstOrDefaultAsync(_ => this.idMapper(_).Equals(id));
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>Task which asynchronously perform operation.</returns>
        public async Task DeleteAsync(TEntity entity)
        {
            this.ThrowIfDisposed();
            this.context.Entry(entity).State = EntityState.Deleted;
            await this.SaveAsync();
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.context.Dispose();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns>Task which asynchronously save changes.</returns>
        private async Task SaveAsync()
        {
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Throws exception if object is disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}
