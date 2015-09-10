// -----------------------------------------------------------------------
// <copyright file="MessageServices.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
#if !NOMAIL
    using System.Net.Mail;
#endif
    using System.Threading.Tasks;

#if NETCORE
    /// <summary>
    /// Message services.
    /// </summary>
    public class MessageServices
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="email">Recepient's email.</param>
        /// <param name="subject">Email subject</param>
        /// <param name="message">Email message text.</param>
        /// <returns>Task which asynchronously send email.</returns>
        public static async Task SendEmailAsync(string email, string subject, string message)
        {
#if !NOMAIL
            SmtpClient client = new SmtpClient("smtp.sendgrid.net");
            client.Credentials = new System.Net.NetworkCredential("azure_246b6e0fe9ab9e8997340a3bc3b8e480@azure.com", "Xp8YZpA_");
            var template = System.IO.File.ReadAllText("mailtemplate.html");
            await client.SendMailAsync("info@hotlola.com", email, subject, template.Replace("$message", message));
#else
            // Plug in your email service
            await Task.FromResult(0);
#endif
        }

        public static Task SendSmsAsync(string number, string message)
        {
            // Plug in your sms service
            return Task.FromResult(0);
        }
    }
#endif
}
