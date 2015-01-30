// -----------------------------------------------------------------------
// <copyright file="ApiStatusResponse.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models
{
    using Dub.Web.Core;

    /// <summary>
    /// Response for the API operation.
    /// </summary>
    public class ApiStatusResponse
    {
        /// <summary>
        /// Gets or sets status code of operation execution.
        /// </summary>
        public ApiStatusCode Code { get; set; }
    }
}
