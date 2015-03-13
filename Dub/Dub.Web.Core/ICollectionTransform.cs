// -----------------------------------------------------------------------
// <copyright file="ICollectionTransform.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Perform transforming of the collections.
    /// </summary>
    /// <typeparam name="T">Type of elements in the filtered collection.</typeparam>
    public interface ICollectionTransform<T>
    {
        /// <summary>
        /// Performs transformation of the collection.
        /// </summary>
        /// <param name="collection">Collection to transform.</param>
        /// <returns>Transformed collection.</returns>
        IEnumerable<object> Transform(IEnumerable<T> collection);
    }
}
