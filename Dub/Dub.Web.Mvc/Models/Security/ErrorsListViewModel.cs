// -----------------------------------------------------------------------
// <copyright file="ErrorsListViewModel.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Models.Security
{
    using System.Linq;
    using Dub.Web.Core;

    /// <summary>
    /// View model for the errors lists.
    /// </summary>
    public class ErrorsListViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsListViewModel"/> class.
        /// </summary>
        /// <param name="items">Items to which should be applied filter.</param>
        /// <param name="filter">Filter which should be applied to the items.</param>
        public ErrorsListViewModel(IQueryable<ErrorLog> items, ErrorsListFilter filter)
        {
            if (filter == null)
            {
                this.Items = items;
                this.Filter = new ErrorsListFilter();
            }
            else
            {
                this.Items = filter.Apply(items);
                this.Filter = filter;
            }
        }

        /// <summary>
        /// Gets or sets items to display.
        /// </summary>
        public IQueryable<ErrorLog> Items { get; set; }

        /// <summary>
        /// Gets filter which applied to the items.
        /// </summary>
        public ErrorsListFilter Filter { get; private set; }
    }
}
