// -----------------------------------------------------------------------
// <copyright file="SecurityController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers
{
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using AutoMapper;
    using Dub.Web.Core;
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.Security;
    using Microsoft.AspNet.Identity.Owin;

    /// <summary>
    /// Controller for managing security related stuff.
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class SecurityController : Controller
    {
        /// <summary>
        /// Initializes static members of the <see cref="SecurityController"/> class.
        /// </summary>
        static SecurityController()
        {
            Mapper.CreateMap<ErrorLog, ErrorLogViewModel>();
        }

        /// <summary>
        /// Display list of log entries.
        /// </summary>
        /// <param name="filter">Filter which should be applied to the log entries.</param>
        /// <returns>Return action result.</returns>
        public ActionResult Errors([Bind(Prefix = "Filter")]ErrorsListFilter filter)
        {
            var dbContext = new ErrorsModel();
            var model = new ErrorsListViewModel(dbContext.ErrorLogs, filter);
            return this.View(model);
        }

        /// <summary>
        /// Display error log entries.
        /// </summary>
        /// <param name="id">Id of the entry to look.</param>
        /// <returns>Task which asynchronously return action result.</returns>
        public async Task<ActionResult> ErrorDetail(int id)
        {
            var dbContext = new ErrorsModel();
            var logEntry = await dbContext.ErrorLogs.FindAsync(id);
            var model = new ErrorLogViewModel();
            Mapper.Map(logEntry, model);
            return this.View(model);
        }
    }
}
