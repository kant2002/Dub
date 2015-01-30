// -----------------------------------------------------------------------
// <copyright file="ApiControllerBase.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Dub.Web.Core;
    using Dub.Web.Mvc.Models;

    /// <summary>
    /// Base controller for all API controllers.
    /// </summary>
    public class ApiControllerBase : ApiController
    {
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
    }
}
