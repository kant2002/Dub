// -----------------------------------------------------------------------
// <copyright file="ErrorLog.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    /// <summary>
    /// Entity which represents error.
    /// </summary>
    public class ErrorLog
    {
        /// <summary>
        /// Gets or sets id of the error log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets name of the user which perform operation.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets date when operation was performed.
        /// </summary>
        public System.DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets message which describe error.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
