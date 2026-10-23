using Farewell.Abstractions.General;
using Farewell.Abstractions.Presentation;

namespace Farewell.Abstractions.Extensions;

public static class ResultCodesPresentationBuilderExtensions
{
    public static IPresentationBuilder AddResultCodes(this IPresentationBuilder builder)
    {
        builder.AddStated(PresentationContext.Default, (ResultCode code) => code)
            .When(ResultCode.Ok, "Operation completed successfully")
            .When(ResultCode.NotFound, "Resource not found")
            .When(ResultCode.Conflict, "Operation conflicts with current state")
            .When(ResultCode.Timeout, "Operation timed out")
            .When(ResultCode.Forbidden, "Operation is forbidden")
            // ♿♿♿ Invalidity team ♿♿♿
            .When(ResultCode.InvalidReference, "Invalid reference")
            .When(ResultCode.InvalidState, "Operation is not valid for the current state")
            .When(ResultCode.InvalidOperation, "Operation cannot be performed")
            .When(ResultCode.InvalidRequest, "Request is invalid for this operation")
            .When(ResultCode.InvalidBusiness, "Operation violates business rules")
            .When(ResultCode.InvalidCustom, "Operation is invalid")
            // Validation
            .When(ResultCode.Validation, "Validation failed")
            .When(ResultCode.ValidationMultipleErrors, "Multiple validation errors occurred")
            .When(ResultCode.ValidationNotEmpty, "Value must not be empty")
            .When(ResultCode.ValidationRequired, "Value is required")
            .When(ResultCode.ValidationMaxLength, "Value exceeds maximum length")
            .When(ResultCode.ValidationMinLength, "Value is shorter than minimum length")
            .When(ResultCode.ValidationInvalidFormat, "Value has an invalid format")
            .When(ResultCode.ValidationInvalidEmail, "Invalid email address")
            .When(ResultCode.ValidationInvalidPhone, "Invalid phone number")
            .When(ResultCode.ValidationMustBePositive, "Value must be positive")
            .When(ResultCode.ValidationMustBeNegative, "Value must be negative")
            .When(ResultCode.ValidationOutOfRange, "Value is out of allowed range")
            .When(ResultCode.ValidationBusinessCheck, "Value violates a business validation rule")
            .When(ResultCode.ValidationCustomCheck, "Value failed a custom validation check")
            // Authorisation - can you do something
            .When(ResultCode.Authorisation, "Not authorised to perform this action")
            .When(ResultCode.AuthorityInsufficientRole, "Insufficient role to perform this action")
            .When(ResultCode.AuthorityInsufficientPermission, "Insufficient permissions")
            .When(ResultCode.AuthorityResourceOwnershipViolation,
                "You do not own the requested resource")
            .When(ResultCode.AuthorityLicenseExpired, "License has expired")
            .When(ResultCode.AuthorityLicenseFeatureUnavailable,
                "Feature is not available in your license")
            .When(ResultCode.AuthorityApprovalRequired, "Action requires approval")
            .When(ResultCode.AuthorityDelegationTokenInvalid, "Delegation token is invalid")
            .When(ResultCode.AuthorityIpPolicyViolation, "Access denied by IP policy")
            .When(ResultCode.AuthorityConcurrentSessionLimitExceeded,
                "Concurrent session limit exceeded")
            .When(ResultCode.AuthorityTimePolicyViolation, "Action not allowed at this time")
            .When(ResultCode.AuthorityCrossBoundaryAccessDenied, "Cross-boundary access denied")
            .When(ResultCode.AuthorityActionForbiddenInWorkflow,
                "Action is forbidden in the current workflow")
            .When(ResultCode.AuthorityBusiness, "Action denied by a business authorisation rule")
            .When(ResultCode.AuthorityCustom, "Action denied by a custom authorisation rule")
            // Authentification - who are you
            .When(ResultCode.Authentification, "Authentication failed")
            .When(ResultCode.AuthCredentials, "Invalid credentials")
            .When(ResultCode.AuthTokenExpired, "Authentication token has expired")
            .When(ResultCode.AuthTokenRevoked, "Authentication token has been revoked")
            .When(ResultCode.AuthTokenMalformed, "Authentication token is malformed")
            .When(ResultCode.AuthAccountLocked, "Account is locked")
            .When(ResultCode.AuthAccountSuspended, "Account is suspended")
            .When(ResultCode.AuthAccountNotActivated, "Account is not activated")
            .When(ResultCode.AuthBusiness, "Authentication rejected by a business rule")
            .When(ResultCode.AuthCustom, "Authentication rejected by a custom rule")
            // File system
            .When(ResultCode.File, "File system operation failed")
            .When(ResultCode.FileNotFound, "File not found")
            .When(ResultCode.FileNotEmpty, "File is not empty")
            .When(ResultCode.FileAlreadyExists, "File already exists")
            .When(ResultCode.FileFileTooLarge, "File is too large")
            .When(ResultCode.FileFileTooSmall, "File is too small")
            .When(ResultCode.FileInvalidFormat, "File has an invalid format")
            .When(ResultCode.FileCorrupted, "File is corrupted")
            .When(ResultCode.FileLocked, "File is locked")
            .When(ResultCode.FileReadOnly, "File is read-only")
            .When(ResultCode.FileInUse, "File is in use")
            .When(ResultCode.FileAccessDenied, "Access to file denied")
            .When(ResultCode.FileInvalidName, "File name is invalid")
            .When(ResultCode.FileDirectoryNotFound, "Directory not found")
            .When(ResultCode.FileDirectoryNotEmpty, "Directory is not empty")
            .When(ResultCode.FileBusiness, "File operation rejected by a business rule")
            .When(ResultCode.FileCustom, "File operation rejected by a custom rule")
            // Business
            .When(ResultCode.Business, "Business rule violated")
            .When(ResultCode.BusinessStateTransition, "Invalid state transition")
            .When(ResultCode.BusinessEntityInUse, "Entity is currently in use")
            .When(ResultCode.BusinessKeyDuplicate, "Duplicate key")
            .When(ResultCode.BusinessConditionNotMet, "Required condition is not met")
            .When(ResultCode.BusinessOperationAlreadyCompleted,
                "Operation has already been completed")
            .When(ResultCode.BusinessInsufficientFunds, "Insufficient funds")
            .When(ResultCode.BusinessDeadlineExceeded, "Deadline exceeded")
            .When(ResultCode.BusinessCircularReference, "Circular reference detected")
            .When(ResultCode.BusinessChildEntityLimitExceeded, "Child entity limit exceeded")
            .When(ResultCode.BusinessDependencyMissing, "Required dependency is missing")
            .When(ResultCode.BusinessLifecyclePhaseViolation, "Lifecycle phase violation")
            .When(ResultCode.BusinessTemporalConflict, "Temporal conflict detected")
            .When(ResultCode.BusinessInvariantViolation, "Invariant violated")
            .When(ResultCode.BusinessNotAllowed, "Operation is not allowed")
            .When(ResultCode.BusinessReversalNotAllowed,
                "Reversal of this operation is not allowed")
            .When(ResultCode.BusinessBusiness, "Operation rejected by a business rule")
            .When(ResultCode.BusinessCustom, "Operation rejected by a custom rule")
            .Else("Unknown result code");

        return builder;
    }
}