// -----------------------------------------------------------------------
// <copyright file="LogApiExceptionAttribute.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;
    using Dub.Web.Core;

    /// <summary>
    /// Represents an attribute that is used to log an exception that is thrown
    /// by an action method.
    /// </summary>
    public class LogApiExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
                var routeData = actionExecutedContext.ActionContext.RequestContext.RouteData;
                string controller = routeData.Values["controller"].ToString();
                string action = routeData.Values["action"].ToString();
                var userName = actionExecutedContext.ActionContext.RequestContext.Principal.Identity.Name;
                ExceptionHelper.PublishException(userName, actionExecutedContext.Exception);
            }
        }

        /// <summary>
        /// Raises the exception event asynchronously.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        /// <param name="cancellationToken">Cancellation token for the processing exception.</param>
        /// <returns>A task representing the asynchronous exception logging operation.</returns>
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
                var routeData = actionExecutedContext.ActionContext.RequestContext.RouteData;
                string controller = routeData.Values["controller"].ToString();
                string action = routeData.Values["action"].ToString();
                var userName = actionExecutedContext.ActionContext.RequestContext.Principal.Identity.Name;
                await ExceptionHelper.PublishExceptionAsync(userName, actionExecutedContext.Exception, cancellationToken);
            }
        }
    }
}
