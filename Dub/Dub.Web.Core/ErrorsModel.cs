// -----------------------------------------------------------------------
// <copyright file="ErrorsModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System.Data.Entity;

    /// <summary>
    /// Database model for the errors handling.
    /// </summary>
    public class ErrorsModel : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsModel" /> class.
        /// </summary>
        public ErrorsModel()
            : base("name=ErrorsModel")
        {
        }

        /// <summary>
        /// Gets or sets error logs which belongs to the given context.
        /// </summary>
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}