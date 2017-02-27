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

        /// <summary>
        /// Login for this account is disallowed.
        /// </summary>
        LoginNotAllowedError = RangeAccount + 7,
        
        /// <summary>
        /// Indicating user lockout is not enabled.
        /// </summary>
        UserLockoutNotEnabled = RangeAccount + 8,
        
        /// <summary>
        /// Indicating a user already has a password.
        /// </summary>
        UserAlreadyHasPassword = RangeAccount + 9,
        
        /// <summary>
        /// Idicating a concurrency failure.
        /// </summary>
        ConcurrencyFailure = RangeAccount + 10,
        
        /// <summary>
        /// Idicating an invalid token.
        /// </summary>
        InvalidToken = RangeAccount + 11,
        
        /// <summary>
        /// Idicating a password mismatch.
        /// </summary>
        PasswordMismatch = RangeAccount + 12,
        
        /// <summary>
        /// Idicating a password entered does not contain a numeric character, 
        /// which is required by the password policy..
        /// </summary>
        PasswordRequiresDigit = RangeAccount + 13,
        
        /// <summary>
        /// Indicating a password entered does not contain a lower case letter, 
        /// which is required by the password policy.
        /// </summary>
        PasswordRequiresLower = RangeAccount + 14,
        
        /// <summary>
        /// Indicating a password entered does not contain a non-alphanumeric character, 
        /// which is required by the password policy.
        /// </summary>
        PasswordRequiresNonAlphanumeric = RangeAccount + 15,
        
        /// <summary>
        /// Indicating a password entered does not contain an upper case letter, 
        /// which is required by the password policy.
        /// </summary>
        PasswordRequiresUpper = RangeAccount + 15,
        
        /// <summary>
        /// Idicating an external login is already associated with an account.
        /// </summary>
        LoginAlreadyAssociated = RangeAccount + 16,
    }
}
