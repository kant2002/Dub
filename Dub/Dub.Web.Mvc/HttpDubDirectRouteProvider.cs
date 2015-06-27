// -----------------------------------------------------------------------
// <copyright file="HttpDubDirectRouteProvider.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
#if !NETCORE
    using System.Collections.Generic;
    using System.Web.Http.Controllers;
    using System.Web.Http.Routing;

    /// <summary>
    /// WebAPI route provider which is required for Dub API controllers to work.
    /// </summary>
    public class HttpDubDirectRouteProvider : DefaultDirectRouteProvider
    {
        /// <summary>
        /// Gets a set of route factories for the given action descriptor.
        /// </summary>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>A set of route factories.</returns>
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(true);
        }
    }
#endif
}
