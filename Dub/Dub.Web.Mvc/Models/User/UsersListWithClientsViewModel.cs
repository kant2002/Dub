// -----------------------------------------------------------------------
// <copyright file="UsersListWithClientsViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.User
{
    using System.Linq;
    using Dub.Web.Identity;

    /// <summary>
    /// View model for the user lists.
    /// </summary>
    public class UsersListWithClientsViewModel
    {
        /// <summary>
        /// Gets or sets users.
        /// </summary>
        public IQueryable<DubUserWithClient> Users { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to display clients to which user belongs.
        /// </summary>
        public bool DisplayClients { get; set; }
    }
}
