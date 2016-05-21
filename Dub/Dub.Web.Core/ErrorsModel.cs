// -----------------------------------------------------------------------
// <copyright file="ErrorsModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
#if !NETCORE
    using System.Data.Entity;
#else
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
#endif

    /// <summary>
    /// Database model for the errors handling.
    /// </summary>
    public class ErrorsModel : DbContext
    {
#if !NETCORE
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsModel" /> class.
        /// </summary>
        public ErrorsModel()
            : base("name=ErrorsModel")
        {
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsModel" /> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public ErrorsModel(DbContextOptions options)
            : base(options)
        {
        }
#endif

        /// <summary>
        /// Gets or sets error logs which belongs to the given context.
        /// </summary>
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}