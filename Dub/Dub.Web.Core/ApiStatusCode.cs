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
        /// Range for the generic codes.
        /// </summary>
        RangeGeneric = 0x0,

        /// <summary>
        /// Range for the account related errors.
        /// </summary>
        RangeAccount = 0x10000000,

        /// <summary>
        /// Successful operation.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Generic operation failure.
        /// </summary>
        OperationFailed = RangeGeneric + 1,

        /// <summary>
        /// Invalid parameters passed to the operation.
        /// </summary>
        InvalidArguments = RangeGeneric + 2,

        /// <summary>
        /// User is locked out.
        /// </summary>
        AccountLockedOut = RangeAccount + 1,

        /// <summary>
        /// Additional verification is required.
        /// </summary>
        AccountRequiresVerification = RangeAccount + 2,

        /// <summary>
        /// Invalid authorization parameters passed.
        /// </summary>
        AuthorizationFailure = RangeAccount + 3,

        /// <summary>
        /// Invalid verification code given.
        /// </summary>
        InvalidVerificationCode = RangeAccount + 4,

        /// <summary>
        /// Registration is failed.
        /// </summary>
        RegistrationFailed = RangeAccount + 5,

        /// <summary>
        /// Removing associated login from account failed.
        /// </summary>
        RemoveLoginError = RangeAccount + 6,
    }
}
