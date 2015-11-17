// -----------------------------------------------------------------------
// <copyright file="ErrorsListFilter.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Security
{
    using System;
    using System.ComponentModel.DataAnnotations;
#if !NETCORE
    using System.Data.Entity.SqlServer;
#endif
    using System.Linq;
    using Dub.Web.Core;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// Filter for the error log items
    /// </summary>
    public class ErrorsListFilter
    {
        /// <summary>
        /// Gets or sets beginning date from which search for requests.
        /// </summary>
        [Display(Name = "FilterFromDate", ResourceType = typeof(Resources))]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Gets or sets ending date from which search for requests.
        /// </summary>
        [Display(Name = "FilterTillDate", ResourceType = typeof(Resources))]
        [DataType(DataType.Date)]
        public DateTime? TillDate { get; set; }

        /// <summary>
        /// Apply parameters specified by this filter to the sequence of data.
        /// </summary>
        /// <param name="source">Source sequence to which filter should be applied.</param>
        /// <returns>Filtered sequence.</returns>
        public IQueryable<ErrorLog> Apply(IQueryable<ErrorLog> source)
        {
            if (this.FromDate.HasValue)
            {
                var fromDate = this.FromDate.GetValueOrDefault(DateTime.MinValue);
#if !NETCORE
                source = source.Where(_ => SqlFunctions.DateDiff("minute", _.Created, fromDate) <= 0);
#else
                fromDate = fromDate.AddSeconds(-fromDate.Second);
                fromDate = fromDate.AddMilliseconds(-fromDate.Millisecond);
                source = source.Where(_ => _.Created >= fromDate);
#endif
            }

            if (this.TillDate.HasValue)
            {
                var tillDate = this.TillDate.GetValueOrDefault(DateTime.MinValue);
#if !NETCORE
                source = source.Where(_ => SqlFunctions.DateDiff("minute", _.Created, tillDate) >= 0);
#else
                tillDate = tillDate.AddSeconds(-tillDate.Second);
                tillDate = tillDate.AddMilliseconds(-tillDate.Millisecond);
                source = source.Where(_ => _.Created <= tillDate);
#endif
            }

            return source;
        }
    }
}
