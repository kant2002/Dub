// -----------------------------------------------------------------------
// <copyright file="ApiErrorStatusResponse.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Response object which contains errors
    /// </summary>
    public class ApiErrorStatusResponse : ApiStatusResponse
    {
        /// <summary>
        /// Gets or sets error codes.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "The property is used as DTO.")]
        public string[] Errors { get; set; }
    }
}
