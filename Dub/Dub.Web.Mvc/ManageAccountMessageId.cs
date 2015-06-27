// -----------------------------------------------------------------------
// <copyright file="ManageAccountMessageId.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    /// <summary>
    /// Enumeration for the available messages during managing user account.
    /// </summary>
    public enum ManageAccountMessageId
    {
        /// <summary>
        /// Message represents that phone added successfully.
        /// </summary>
        AddPhoneSuccess,

        /// <summary>
        /// Message represents that password changed successfully.
        /// </summary>
        ChangePasswordSuccess,

        /// <summary>
        /// Message represents that two-factor authentication provider sets successfully.
        /// </summary>
        SetTwoFactorSuccess,

        /// <summary>
        /// Message represents that password set successfully.
        /// </summary>
        SetPasswordSuccess,

        /// <summary>
        /// Message represents that external login added successfully.
        /// </summary>
        AddLoginSuccess,

        /// <summary>
        /// Message represents that external login removed successfully.
        /// </summary>
        RemoveLoginSuccess,

        /// <summary>
        /// Message represents that phone removed successfully.
        /// </summary>
        RemovePhoneSuccess,

        /// <summary>
        /// Message represents that error happens.
        /// </summary>
        Error
    }
}
