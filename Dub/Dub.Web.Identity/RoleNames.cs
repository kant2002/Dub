// -----------------------------------------------------------------------
// <copyright file="RoleNames.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    /// <summary>
    /// Constants for the role names
    /// </summary>
    public class RoleNames
    {
        /// <summary>
        /// Name of the administrator role.
        /// </summary>
        public const string Administrator = "Administrator";

        /// <summary>
        /// Id of the administrator role.
        /// </summary>
        public const string AdministratorId = "d9317a83-834b-46c6-8df2-2c1438adf390";

        /// <summary>
        /// Name of the manager role.
        /// </summary>
        public const string ClientAdministrator = "ClientAdministrator";

        /// <summary>
        /// Id of the manager role.
        /// </summary>
        public const string ManagerId = "d9317a83-834b-46c6-8df2-2c1438adf391";

        /// <summary>
        /// Administrator and manager roles together.
        /// </summary>
        public const string AdministratorAndManager = Administrator + "," + ClientAdministrator;
    }
}
