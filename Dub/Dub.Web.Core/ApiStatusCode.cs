// -----------------------------------------------------------------------
// <copyright file="ApiStatusCode.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Core
{
    /// <summary>
    /// Error codes for the operations.
    /// </summary>
    public enum ApiStatusCode
    {
        /// <summary>
        /// Successful operation.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// User is locked out.
        /// </summary>
        AccountLockedOut = 1,

        /// <summary>
        /// Additional verification is required.
        /// </summary>
        AccountRequiresVerification = 2,

        /// <summary>
        /// Invalid authorization parameters passed.
        /// </summary>
        AuthorizationFailure = 3,

        /// <summary>
        /// Invalid verification code given.
        /// </summary>
        InvalidVerificationCode = 4,

        /// <summary>
        /// Registration is failed.
        /// </summary>
        RegistrationFailed = 5,

        /// <summary>
        /// Operation is failed.
        /// </summary>
        OperationFailed = 6,
    }
}
