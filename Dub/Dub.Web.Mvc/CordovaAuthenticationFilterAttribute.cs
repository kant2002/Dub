// -----------------------------------------------------------------------
// <copyright file="CordovaAuthenticationFilterAttribute.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if! NETCORE
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Microsoft.Owin.Security.DataProtection;
#endif
#if NETCORE
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc.Filters;
#endif
#if !NETCORE
    using AuthorizationFilterContext = System.Web.Http.Controllers.HttpActionContext;
#endif

    /// <summary>
    /// Authentication filter which use custom header for sending authentication token.
    /// </summary>
    public class CordovaAuthenticationFilterAttribute
#if NETCORE
        : IAuthorizationFilter
#else
        : AuthorizationFilterAttribute
#endif
    {
        /// <summary>
        /// Custom header for the API key.
        /// </summary>
        public const string DefaultAuthenticationHeader = "X-Auth-Token";

        /// <summary>
        /// Initializes a new instance of the <see cref="CordovaAuthenticationFilterAttribute"/> class.
        /// </summary>
        /// <param name="header">HTTP header used for the delivering authentication header.</param>
        public CordovaAuthenticationFilterAttribute(string header = DefaultAuthenticationHeader)
        {
            this.AuthenticationHeader = header;
        }

        /// <summary>
        /// Gets or sets authentication header which used for delivering security token.
        /// </summary>
        public string AuthenticationHeader { get; set; }

#if NETCORE
        /// <summary>
        /// Calls when a process requests authorization.
        /// </summary>
        /// <param name="actionContext">The action context, which encapsulates information for using <see cref="AuthorizationFilterAttribute"/>.</param>
        public void OnAuthorization(AuthorizationFilterContext actionContext)
        {
        }
#else
        /// <summary>
        /// Calls when a process requests authorization.
        /// </summary>
        /// <param name="actionContext">The action context, which encapsulates information for using <see cref="AuthorizationFilterAttribute"/>.</param>
        public override void OnAuthorization(AuthorizationFilterContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }
#endif
    }
}
