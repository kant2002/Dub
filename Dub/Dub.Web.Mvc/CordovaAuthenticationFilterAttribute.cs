namespace Dub.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Microsoft.Owin.Security.DataProtection;

    /// <summary>
    /// Authentication filter which use custom header for sending authentication token.
    /// </summary>
    public class CordovaAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Custom header for the API key.
        /// </summary>
        public const string DefaultAuthenticationHeader = "X-Auth-Token";

        /// <summary>
        /// Initialize a new instance of the <see cref="CordovaAuthenticationFilterAttribute"/> class.
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

        /// <summary>
        /// Calls when a process requests authorization.
        /// </summary>
        /// <param name="actionContext">The action context, which encapsulates information for using <see cref="AuthorizationFilterAttribute"/>.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }
    }
}
