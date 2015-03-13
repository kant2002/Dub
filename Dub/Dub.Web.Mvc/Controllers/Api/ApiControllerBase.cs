// -----------------------------------------------------------------------
// <copyright file="ApiControllerBase.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Dub.Web.Core;
    using Dub.Web.Mvc.Models;
    using Microsoft.Owin;

    /// <summary>
    /// Base controller for all API controllers.
    /// </summary>
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// Gets OWIN context.
        /// </summary>
        protected IOwinContext OwinContext
        {
            get
            {
                return HttpContext.Current.GetOwinContext();
            }
        }

        /// <summary>
        /// Returns  simple API status code.
        /// </summary>
        /// <param name="code">API code to return.</param>
        /// <returns>Action result which represent specific API code.</returns>
        protected IHttpActionResult StatusCode(ApiStatusCode code)
        {
            return this.Ok(new ApiStatusResponse() { Code = code });
        }

        /// <summary>
        /// Returns API status code which contains information about errors.
        /// </summary>
        /// <param name="code">API code to return.</param>
        /// <param name="errors">Sequence of errors.</param>
        /// <returns>Action result which represent specific API code.</returns>
        protected IHttpActionResult ErrorCode(ApiStatusCode code, IEnumerable<string> errors)
        {
            return this.Ok(new ApiErrorStatusResponse() 
            { 
                Code = code, 
                Errors = errors.ToArray() 
            });
        }

        /// <summary>
        /// Performs filtering of the data collection.
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection to filter.</typeparam>
        /// <param name="collection">Sequence of elements to filter.</param>
        /// <param name="filter">Filter which apply to collection.</param>
        /// <param name="sortBy">Sorting function.</param>
        /// <param name="sortAscending">Sorting direction.</param>
        /// <param name="startRow">Row from which start filtering.</param>
        /// <param name="pageSize">Count of rows in the page.</param>
        /// <returns>Collection with specific filtering rules applied.</returns>
        protected IEnumerable<T> Filter<T>(
            IQueryable<T> collection, 
            ICollectionFilter<T> filter, 
            Func<T, IComparable> sortBy, 
            bool sortAscending, 
            int startRow, 
            int pageSize)
        {
            collection = filter.Filter(collection);
            if (sortBy == null)
            {
                return collection;
            }

            // Perform sorting
            var sortedCollection = sortAscending
                ? collection.OrderBy(sortBy)
                : collection.OrderByDescending(sortBy);
            
            // Perform filtering.
            var pagedCollection = sortedCollection.Skip(startRow).Take(pageSize);
            return pagedCollection;
        }
    }
}
