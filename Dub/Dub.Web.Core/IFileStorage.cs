// -----------------------------------------------------------------------
// <copyright file="IFileStorage.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for persisting file.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Saves the file to the file storage.
        /// </summary>
        /// <param name="area">Area of the file to save.</param>
        /// <param name="originalFileName">Name of the original file to save.</param>
        /// <param name="file">File stream to store.</param>
        /// <returns>Task which perform operation.</returns>
        Task<string> Save(string area, string originalFileName, Stream file);
    }
}
