// -----------------------------------------------------------------------
// <copyright file="SimpleFileStorage.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// File storage which saves iamges in the local file system.
    /// </summary>
    public class SimpleFileStorage : IFileStorage
    {
        private string basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFileStorage"/> class.
        /// </summary>
        /// <param name="basePath">Base path for storing images.</param>
        /// <param name="wellKnownLocations">List of well known locations.</param>
        public SimpleFileStorage(string basePath, string[] wellKnownLocations)
        {
            this.basePath = basePath;
            foreach (var locationName in wellKnownLocations)
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(this.basePath, locationName));
                }
                catch (UnauthorizedAccessException)
                {
                    // do nothing.
                }
            }
        }

        /// <summary>
        /// Saves the file to the file storage.
        /// </summary>
        /// <param name="area">Area of the file to save.</param>
        /// <param name="originalFileName">Name of the original file to save.</param>
        /// <param name="file">File stream to store.</param>
        /// <returns>Task which perform operation.</returns>
        public async Task<string> Save(string area, string originalFileName, Stream file)
        {
            var uploadPath = Path.Combine(this.basePath, area);
            var extension = Path.GetExtension(originalFileName);
            var fileName = GetUniqueFileName(extension, uploadPath);
            var path = Path.GetFullPath(Path.Combine(uploadPath, fileName));
            using (var writer = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(writer);
            }

            return fileName;
        }

        private static string GetUniqueFileName(string extension, string path)
        {
            var nameUnique = true;
            string fileName;
            do
            {
                fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, extension);

                nameUnique = !File.Exists(Path.Combine(path, fileName));
            }
            while (!nameUnique);
            return fileName;
        }
    }
}
