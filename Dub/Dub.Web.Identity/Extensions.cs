// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Identity
{
    using System;
    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Extension helpers.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Configure user manager.
        /// </summary>
        /// <typeparam name="TUser">Type of the user.</typeparam>
        /// <param name="manager">User manager which has to be configured.</param>
        /// <param name="emailDisplayName">Display name for the email service.</param>
        public static void ConfigureUserManager<TUser>(this DubUserManager<TUser> manager, string emailDisplayName)
            where TUser : DubUser
        {
            manager.UserValidator = new UserValidator<TUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            var phoneProvider = new PhoneNumberTokenProvider<TUser>
            {
                MessageFormat = "Your security code is {0}"
            };
            var emailProvider = new EmailTokenProvider<TUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            };
            manager.RegisterTwoFactorProvider("Phone Code", phoneProvider);
            manager.RegisterTwoFactorProvider("Email Code", emailProvider);
            manager.EmailService = new MarkdownEmailService(emailDisplayName);
            manager.SmsService = new EmptySmsService();
        }
    }
}
