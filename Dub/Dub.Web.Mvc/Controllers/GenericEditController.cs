﻿// -----------------------------------------------------------------------
// <copyright file="GenericEditController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web;
    using System.Web.Mvc;
#endif
    using Dub.Web.Core;
#if !NETCORE
    using Microsoft.AspNet.Identity.Owin;
#endif
#if NETCORE
    using Microsoft.AspNetCore.Mvc;
#endif

    /// <summary>
    /// Generic controller for editing.
    /// </summary>
    /// <typeparam name="TEntityKey">Type of key for the entity.</typeparam>
    /// <typeparam name="TEntity">Type of entity to be managed.</typeparam>
    public class GenericEditController<TEntityKey, TEntity> : Controller
        where TEntity : class, IHasKey<TEntityKey>, new()
        where TEntityKey : IEquatable<TEntityKey>
    {
        /// <summary>
        /// Create new instance of the <see cref="IGenericStore{TEntityKey, TEntity}"/> class.
        /// </summary>
        /// <param name="store"></param>
        public GenericEditController(IGenericStore<TEntityKey, TEntity> store)
        {
            this.Store = store;
        }

        /// <summary>
        /// Gets persistence store.
        /// </summary>
        protected IGenericStore<TEntityKey, TEntity> Store { get; private set; }

        /// <summary>
        /// Displays list of all entities.
        /// </summary>
        /// <returns>Result of the action.</returns>
        public ActionResult Index()
        {
            var store = this.Store;
            return this.View(store.Items);
        }

        /// <summary>
        /// Show create entity page.
        /// </summary>
        /// <returns>Result of the action.</returns>
        public ActionResult Create()
        {
            var model = new TEntity();
            return this.View(model);
        }

        /// <summary>
        /// Saves changes during creation of the entity.
        /// </summary>
        /// <param name="model">Data to use when create entity.</param>
        /// <returns>Task which returns result of the action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TEntity model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.Store.CreateAsync(model);
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Show edit entity page by id.
        /// </summary>
        /// <param name="id">Id of the entity to get</param>
        /// <returns>Task which returns result of the action.</returns>
        public async Task<ActionResult> Edit(TEntityKey id)
        {
            var model = await this.Store.FindByIdAsync(id);
            if (model == null)
            {
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        /// <summary>
        /// Saves changes during editing of the user.
        /// </summary>
        /// <param name="model">Data to save about user.</param>
        /// <returns>Task which returns result of the action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TEntity model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.Store.UpdateAsync(model);
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Show delete prompt page for entity with given id.
        /// </summary>
        /// <param name="id">Id of the entity to prompt for deletion</param>
        /// <returns>Task which returns result of the action.</returns>
        public async Task<ActionResult> Delete(TEntityKey id)
        {
            var model = await this.Store.FindByIdAsync(id);
            if (model == null)
            {
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="model">Entity to delete.</param>
        /// <returns>Task which returns result of the action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(TEntity model)
        {
            await this.Store.DeleteAsync(model);
            return this.RedirectToAction("Index");
        }
    }
}
