// -----------------------------------------------------------------------
// <copyright file="ApiErrorStatusResponse.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models
{
    /// <summary>
    /// Response object which contains errors
    /// </summary>
    public class ApiErrorStatusResponse : ApiStatusResponse
    {
        /// <summary>
        /// Gets or sets error codes.
        /// </summary>
        public string[] Errors { get; set; }
    }
}
