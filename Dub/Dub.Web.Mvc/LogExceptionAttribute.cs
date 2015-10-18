// -----------------------------------------------------------------------
// <copyright file="LogExceptionAttribute.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System;
#if !NETCORE
    using System.Web.Mvc;
#endif
    using Dub.Web.Core;
#if NETCORE
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc.Filters;
#endif

#if !NETCORE
    /// <summary>
    /// Represents an attribute that is used to log an exception that is thrown
    /// by an action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class LogExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.Exception != null)
            {
                var routeData = filterContext.RequestContext.RouteData;
                string controller = routeData.Values["controller"].ToString();
                string action = routeData.Values["action"].ToString();
                var userName = filterContext.HttpContext.User.Identity.Name;
                ExceptionHelper.PublishException(userName, filterContext.Exception);
            }
        }
    }
#else
    /// <summary>
    /// Represents an attribute that is used to log an exception that is thrown
    /// by an action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class LogExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
                var userName = actionExecutedContext.HttpContext.User.Identity.Name;
                ExceptionHelper.PublishException(userName, actionExecutedContext.Exception);
            }
        }

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
    }
#endif
}
