// -----------------------------------------------------------------------
// <copyright file="ICollectionFilter.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System.Linq;

    /// <summary>
    /// Filters collection.
    /// </summary>
    /// <typeparam name="T">Type of elements in the collection to filter.</typeparam>
    public interface ICollectionFilter<T>
    {
        /// <summary>
        /// Performs filtering of the collection.
        /// </summary>
        /// <param name="collection">Collection to filter.</param>
        /// <returns>Filtered collection.</returns>
        IQueryable<T> Filter(IQueryable<T> collection);
    }
}
