// -----------------------------------------------------------------------
// <copyright file="LogApiExceptionAttribute.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web.Http.Filters;
#endif
    using Dub.Web.Core;
#if NETCORE
    using Microsoft.AspNet.Mvc;
#endif
#if !NETCORE
    using ExceptionContext = System.Web.Http.Filters.HttpActionExecutedContext;
#endif

    /// <summary>
    /// Represents an attribute that is used to log an exception that is thrown
    /// by an action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class LogApiExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
#if !NETCORE
                var routeData = actionExecutedContext.ActionContext.RequestContext.RouteData;
                string controller = routeData.Values["controller"].ToString();
                string action = routeData.Values["action"].ToString();
                var userName = actionExecutedContext.ActionContext.RequestContext.Principal.Identity.Name;
#else
                var userName = actionExecutedContext.HttpContext.User.Identity.Name;
#endif
                ExceptionHelper.PublishException(userName, actionExecutedContext.Exception);
            }
        }

#if !NETCORE
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
#else
        /// <summary>
        /// Raises the exception event asynchronously.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        /// <returns>A task representing the asynchronous exception logging operation.</returns>
        public override async Task OnExceptionAsync(ExceptionContext actionExecutedContext)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
                var userName = actionExecutedContext.HttpContext.User.Identity.Name;
                await ExceptionHelper.PublishExceptionAsync(userName, actionExecutedContext.Exception, CancellationToken.None);
            }
        }
#endif
    }
}
