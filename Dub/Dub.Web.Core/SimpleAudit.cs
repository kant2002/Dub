// -----------------------------------------------------------------------
// <copyright file="SimpleAudit.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Complex type which describe simple audit entities in the database.
    /// </summary>
    [ComplexType]
    public class SimpleAudit
    {
        /// <summary>
        /// Gets or sets user name who change entity.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets date when entity was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets user who last time modify entity.
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets date when entity was last time modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }
}
