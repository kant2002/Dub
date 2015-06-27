// -----------------------------------------------------------------------
// <copyright file="ChallengeResult.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System.Web;
#if !NETCORE
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
#endif
    using Microsoft.Owin.Security;

    /// <summary>
    /// Http result which represents challenge procedure.
    /// </summary>
    internal class ChallengeResult : HttpUnauthorizedResult
    {
        /// <summary>
        /// Used for XSRF protection when adding external logins
        /// </summary>
        private const string XsrfKey = "XsrfId";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeResult"/> class using authorization provider and redirect uri.
        /// </summary>
        /// <param name="provider">Authorization provider to use in the challenge</param>
        /// <param name="redirectUri">Redirect uri to which user should be redirected after challenge succeed.</param>
        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeResult"/> class using authorization provider, redirect uri and specific user.
        /// </summary>
        /// <param name="provider">Authorization provider to use in the challenge</param>
        /// <param name="redirectUri">Redirect uri to which user should be redirected after challenge succeed.</param>
        /// <param name="userId">Id of the user for which challenge performed.</param>
        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            this.LoginProvider = provider;
            this.RedirectUri = redirectUri;
            this.UserId = userId;
        }

        /// <summary>
        /// Gets or sets authorization provider to use in the challenge
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets redirect uri to which user should be redirected after challenge succeed.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets id of the user for which challenge performed.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Add information into the response environment that will cause the authentication
        /// middleware to challenge the caller to authenticate. This also changes the
        /// status code of the response to 401. The nature of that challenge varies greatly,
        /// and ranges from adding a response header or changing the 401 status code
        /// to a 302 redirect.
        /// </summary>
        /// <param name="context">
        /// The context in which the result is executed. The context information includes
        /// the controller, HTTP content, request context, and route data.
        /// </param>
        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = this.RedirectUri };
            if (this.UserId != null)
            {
                properties.Dictionary[XsrfKey] = this.UserId;
            }

            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);
        }
    }
}
