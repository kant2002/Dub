// -----------------------------------------------------------------------
// <copyright file="ExceptionHelper.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    using System;
#if !NETCORE
    using System.Data.Entity.Validation;
#endif
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for handling exceptions.
    /// </summary>
    [Obsolete("This class is obsolete, and other logging and instrumentation frameworks recommended to use.")]
    public static class ExceptionHelper
    {
        /// <summary>
        /// Model builder.
        /// </summary>
        private static Func<ErrorsModel> modelBuilder = BuildErrorsModel;

        /// <summary>
        /// Overrides the model builder with new value.
        /// </summary>
        /// <param name="builder">New value for the model builder.</param>
        public static void SetModelBuilder(Func<ErrorsModel> builder)
        {
            modelBuilder = builder;
        }

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
            var context = modelBuilder();
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
            var context = modelBuilder();
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

#if !NETCORE
            var databaseValidationException = ex as DbEntityValidationException;
            if (databaseValidationException != null)
            {
                return FormatException(databaseValidationException);
            }
#endif

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

#if !NETCORE
        /// <summary>
        /// Format exception to string.
        /// </summary>
        /// <param name="ex">Exception which should be converted to string.</param>
        /// <returns>String that represent exception information.</returns>
        public static string FormatException(DbEntityValidationException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

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
#endif

        /// <summary>
        /// Builds the default errors model.
        /// </summary>
        /// <returns>Default errors model built.</returns>
        private static ErrorsModel BuildErrorsModel()
        {
#if !NETCORE
            return new ErrorsModel();
#else
            throw new NotImplementedException("Please implement function which will create your instance of ErrorsModel and use ExceptionHelper.SetModelBuilder to set it as default model builder.");
#endif
        }
    }
}
