// -----------------------------------------------------------------------
// <copyright file="MessageServices.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System.Threading.Tasks;

#if NETCORE
    /// <summary>
    /// Message services.
    /// </summary>
    public class MessageServices
    {
        /// <summary>
        /// Send email 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service
            return Task.FromResult(0);
        }

        public static Task SendSmsAsync(string number, string message)
        {
            // Plug in your sms service
            return Task.FromResult(0);
        }
    }
#endif
}
