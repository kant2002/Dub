// -----------------------------------------------------------------------
// <copyright file="SecurityController.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc.Controllers
{
    using System.Threading.Tasks;
#if !NETCORE
    using System.Web;
    using System.Web.Mvc;
#endif
    using AutoMapper;
    using Dub.Web.Core;
    using Dub.Web.Identity;
    using Dub.Web.Mvc.Models.Security;
#if NETCORE
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
#else
    using Microsoft.AspNet.Identity.Owin;
#endif

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

#if NETCORE
        /// <summary>
        /// Initialize a new instance of the <see cref="SecurityController"/> class.
        /// </summary>
        /// <param name="model">Database context to use.</param>
        public SecurityController(ErrorsModel model)
        {
            this.Model = model;
        }

        /// <summary>
        /// Gets or sets database model to use.
        /// </summary>
        public ErrorsModel Model { get; set; }
#endif

        /// <summary>
        /// Display list of log entries.
        /// </summary>
        /// <param name="filter">Filter which should be applied to the log entries.</param>
        /// <returns>Return action result.</returns>
        public ActionResult Errors([Bind(Prefix = "Filter")]ErrorsListFilter filter)
        {
            var dbContext = this.CreateModel();
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
            var dbContext = this.CreateModel();
#if !NETCORE
            var logEntry = await dbContext.ErrorLogs.FindAsync(id);
#else
            var logEntry = await dbContext.ErrorLogs.FirstOrDefaultAsync(_ => _.Id == id);
#endif
            var model = new ErrorLogViewModel();
            Mapper.Map(logEntry, model);
            return this.View(model);
        }

        /// <summary>
        /// Create new database model.
        /// </summary>
        /// <returns>Database context for error handling.</returns>
        public ErrorsModel CreateModel()
        {
#if !NETCORE
            return new ErrorsModel();
#else
            return this.Model;
#endif
        }
    }
}
