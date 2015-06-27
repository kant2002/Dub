// -----------------------------------------------------------------------
// <copyright file="Client.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
#if !NETCORE
    using System.ComponentModel.DataAnnotations;
#else
    using Microsoft.Data.Entity.Metadata.ModelConventions;
#endif

    /// <summary>
    /// Entity representing client.
    /// </summary>
    /// <remarks>Clients usually is the company which pays for the service.</remarks>
    public partial class Client
    {
        /// <summary>
        /// Gets or sets id of the client in the system.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets name of the company.
        /// </summary>
#if !NETCORE
        [MaxLength(100)]
#endif
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets name of the contact person within organization.
        /// </summary>
#if !NETCORE
        [MaxLength(200)]
#endif
        public string ContactPerson { get; set; }

        /// <summary>
        /// Gets or sets email of the contact person within organization.
        /// </summary>
#if !NETCORE
        [MaxLength(100)]
#endif
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets phone of the contact person within organization.
        /// </summary>
#if !NETCORE
        [MaxLength(100)]
#endif
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets custom notes for the organization.
        /// </summary>
        public string Notes { get; set; }
    }
}
