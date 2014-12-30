// -----------------------------------------------------------------------
// <copyright file="ExceptionHelper.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
    using System.Data.Entity.Validation;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for handling exceptions.
    /// </summary>
    public class ExceptionHelper
    {
        /// <summary>
        /// Publish exceptions async.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="ex">Exception to publish.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task representing the asynchronous exception logging operation.</returns>
        public static async Task PublishExceptionAsync(string userName, Exception ex, CancellationToken cancellationToken)
        {
            var errorMessage = FormatException(ex);
            var context = new ErrorsModel();
            var logEntry = new ErrorLog();
            logEntry.Created = DateTime.UtcNow;
            logEntry.ErrorMessage = errorMessage;
            logEntry.UserName = userName;
            context.ErrorLogs.Add(logEntry);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Publish exceptions.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="ex">Exception to publish.</param>
        public static void PublishException(string userName, Exception ex)
        {
            var errorMessage = FormatException(ex);
            var context = new ErrorsModel();
            var logEntry = new ErrorLog();
            logEntry.Created = DateTime.UtcNow;
            logEntry.ErrorMessage = errorMessage;
            logEntry.UserName = userName;
            context.ErrorLogs.Add(logEntry);
            context.SaveChanges();
        }

        /// <summary>
        /// Format exception to string.
        /// </summary>
        /// <param name="ex">Exception which should be converted to string.</param>
        /// <returns>String that represent exception information.</returns>
        public static string FormatException(Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            var databaseValidationException = ex as DbEntityValidationException;
            if (databaseValidationException != null)
            {
                return FormatException(databaseValidationException);
            }

            var newline = Environment.NewLine;
            var message = ex.Message + newline
                + "Exception type: " + ex.GetType().FullName + newline
                + ex.StackTrace + newline
                + newline;
            if (ex.InnerException != null)
            {
                message += "-----------------------------------------------------------" + newline
                    + newline
                    + "Inner exception: " + newline
                    + FormatException(ex.InnerException);
            }

            return message;
        }

        /// <summary>
        /// Format exception to string.
        /// </summary>
        /// <param name="ex">Exception which should be converted to string.</param>
        /// <returns>String that represent exception information.</returns>
        public static string FormatException(DbEntityValidationException ex)
        {
            var newline = Environment.NewLine;
            StringBuilder validationErrors = new StringBuilder();
            foreach (var entityValidationError in ex.EntityValidationErrors)
            {
                foreach (var validationError in entityValidationError.ValidationErrors)
                {
                    var validationErrorDescription = string.Format("Property {0}. Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    validationErrors.AppendLine(validationErrorDescription);
                }
            }

            var message = ex.Message + newline
                + "Validation errors: " + validationErrors.ToString() + newline
                + "Exception type: " + ex.GetType().FullName + newline
                + ex.StackTrace + newline
                + newline;
            if (ex.InnerException != null)
            {
                message += "-----------------------------------------------------------" + newline
                    + newline
                    + "Inner exception: " + newline
                    + FormatException(ex.InnerException);
            }

            return message;
        }
    }
}
