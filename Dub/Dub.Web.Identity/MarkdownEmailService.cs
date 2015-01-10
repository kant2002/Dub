// -----------------------------------------------------------------------
// <copyright file="MarkdownEmailService.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    using System.Configuration;
    using System.Net.Configuration;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Service for sending email
    /// </summary>
    public class MarkdownEmailService : IIdentityMessageService
    {
        /// <summary>
        /// Display name of the email service.
        /// </summary>
        private string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownEmailService"/> class.
        /// </summary>
        /// <param name="displayName">Display name of the sender.</param>
        public MarkdownEmailService(string displayName)
        {
            this.displayName = displayName;
        }

        /// <summary>
        /// This method should send the message
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <returns>Asynchronous task which sends the message.</returns>
        public async Task SendAsync(IdentityMessage message)
        {
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            var destinationAddress = new MailAddress(message.Destination);
            var fromAddress = smtpSection.From ?? smtpSection.Network.UserName;
            var senderAddress = new MailAddress(fromAddress, this.displayName);
            var textBody = message.Body;
            var htmlBody = CommonMark.CommonMarkConverter.Convert(message.Body);
            var email = new MailMessage(senderAddress, destinationAddress);
            email.Subject = message.Subject;
            email.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(textBody, null, MediaTypeNames.Text.Plain));
            email.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html));

            using (var client = new SmtpClient())
            {
                await client.SendMailAsync(email);
            }
        }
    }
}
