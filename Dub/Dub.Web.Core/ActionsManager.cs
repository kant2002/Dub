// -----------------------------------------------------------------------
// <copyright file="ActionsManager.cs" company="Andrey Kurdiumov">
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
    /// Manage actions for the users.
    /// </summary>
    public class ActionsManager : IDisposable
    {
        /// <summary>
        /// A value indicating that object is disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Action providers registers within this action manager.
        /// </summary>
        private List<IActionProvider> providers = new List<IActionProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionsManager"/> class.
        /// </summary>
        /// <param name="userBuilder">User for which action manager will provide results.</param>
        public ActionsManager(Func<ClaimsPrincipal> userBuilder)
        {
            this.User = new Lazy<ClaimsPrincipal>(userBuilder);
        }

        /// <summary>
        /// Gets user for which actions manager will provide information.
        /// </summary>
        public Lazy<ClaimsPrincipal> User { get; private set; }

        /// <summary>
        /// Get actions for the specific item.
        /// </summary>
        /// <param name="item">Item for which return action descriptions.</param>
        /// <returns>Action descriptions.</returns>
        public IEnumerable<ActionDescription> GetActions(object item)
        {
            this.ThrowIfDisposed();
            var providers = this.GetProviders(item);
            foreach (var provider in providers)
            {
                foreach (var actionDescription in provider.GetActions(this.User.Value, item))
                {
                    yield return actionDescription;
                }
            }
        }

        /// <summary>
        /// Tests whether operation is allowed
        /// </summary>
        /// <param name="commandId">Id of the command which should be tested on the allowance.</param>
        /// <param name="item">Entity on which operation should be performed.</param>
        /// <returns>True if operation on entity is allowed.</returns>
        public bool IsOperationAllowed(string commandId, object item)
        {
            this.ThrowIfDisposed();
            var actions = this.GetActions(item);
            return actions.FirstOrDefault(_ => _.Id == commandId) != null;
        }

        /// <summary>
        /// Tests whether operation is allowed.
        /// </summary>
        /// <typeparam name="T">Type of entity on which operation would be performed.</typeparam>
        /// <param name="commandId">Id of the command which should be tested on the allowance.</param>
        /// <returns>True if operation on entity is allowed.</returns>
        public bool IsOperationAllowed<T>(string commandId)
            where T : class, new()
        {
            this.ThrowIfDisposed();
            return this.IsOperationAllowed(commandId, new T());
        }

        /// <summary>
        /// Register action provider.
        /// </summary>
        /// <param name="provider">Action provider to register.</param>
        public void RegisterProvider(IActionProvider provider)
        {
            this.ThrowIfDisposed();
            this.providers.Add(provider);
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
                this.disposed = true;
            }
        }

        /// <summary>
        /// Gets sequence of registered providers which could add some action to entity.
        /// </summary>
        /// <param name="item">Entity for which get providers.</param>
        /// <returns>Providers which could add some actions for the entity.</returns>
        private IEnumerable<IActionProvider> GetProviders(object item)
        {
            foreach (var provider in this.providers)
            {
                if (provider.IsEntitySupported(item))
                {
                    yield return provider;
                }
            }
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
