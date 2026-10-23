namespace Farewell.Abstractions.General;

/// <summary>
/// Custom codes are defined in area 0_001 - 0_999.
/// </summary>
/// <param name="Code"></param>
public record struct ResultCode(int Code)
{
    public static implicit operator ResultCode(int x) => new(x);
    public static implicit operator int(ResultCode code) => code.Code;

    public static readonly ResultCode Ok = new(0);
    public static readonly ResultCode NotFound = new(1_404);
    public static readonly ResultCode Conflict = new(1_409);
    public static readonly ResultCode Timeout = new(1_504);
    public static readonly ResultCode Forbidden = new(1_403);

    // ♿♿♿ Invalidity team ♿♿♿
    public static readonly ResultCode InvalidReference = new(2_000);

    // current system state cannot handle that operation
    public static readonly ResultCode InvalidState = new(2_001);

    // operation cannot be handled at all
    public static readonly ResultCode InvalidOperation = new(2_002);

    // operation can be handled, but not with that request
    public static readonly ResultCode InvalidRequest = new(2_003);
    public static readonly ResultCode InvalidBusiness = new(2_100);
    public static readonly ResultCode InvalidCustom = new(2_200);

    // Validation
    public static readonly ResultCode Validation = new(3_000);
    public static readonly ResultCode ValidationMultipleErrors = new(3_001);
    public static readonly ResultCode ValidationNotEmpty = new(3_002);
    public static readonly ResultCode ValidationRequired = new(3_003);
    public static readonly ResultCode ValidationMaxLength = new(3_004);
    public static readonly ResultCode ValidationMinLength = new(3_005);
    public static readonly ResultCode ValidationInvalidFormat = new(3_006);
    public static readonly ResultCode ValidationInvalidEmail = new(3_007);
    public static readonly ResultCode ValidationInvalidPhone = new(3_008);
    public static readonly ResultCode ValidationMustBePositive = new(3_009);
    public static readonly ResultCode ValidationMustBeNegative = new(3_010);
    public static readonly ResultCode ValidationOutOfRange = new(3_011);
    public static readonly ResultCode ValidationBusinessCheck = new(3_100);
    public static readonly ResultCode ValidationCustomCheck = new(3_200);

    // Authorisation - can you do something
    public static readonly ResultCode Authorisation = new(4_000);
    public static readonly ResultCode AuthorityInsufficientRole = new(4_001);
    public static readonly ResultCode AuthorityInsufficientPermission = new(4_002);
    public static readonly ResultCode AuthorityResourceOwnershipViolation = new(4_003);
    public static readonly ResultCode AuthorityLicenseExpired = new(4_004);
    public static readonly ResultCode AuthorityLicenseFeatureUnavailable = new(4_005);
    public static readonly ResultCode AuthorityApprovalRequired = new(4_006);
    public static readonly ResultCode AuthorityDelegationTokenInvalid = new(4_007);
    public static readonly ResultCode AuthorityIpPolicyViolation = new(4_008);
    public static readonly ResultCode AuthorityConcurrentSessionLimitExceeded = new(4_009);
    public static readonly ResultCode AuthorityTimePolicyViolation = new(4_010);
    public static readonly ResultCode AuthorityCrossBoundaryAccessDenied = new(4_011);
    public static readonly ResultCode AuthorityActionForbiddenInWorkflow = new(4_012);
    public static readonly ResultCode AuthorityBusiness = new(4_100);
    public static readonly ResultCode AuthorityCustom = new(4_200);

    // Authentification - who are you
    public static readonly ResultCode Authentification = new(5_000);
    public static readonly ResultCode AuthCredentials = new(5_001);
    public static readonly ResultCode AuthTokenExpired = new(5_002);
    public static readonly ResultCode AuthTokenRevoked = new(5_003);
    public static readonly ResultCode AuthTokenMalformed = new(5_004);
    public static readonly ResultCode AuthAccountLocked = new(5_005);
    public static readonly ResultCode AuthAccountSuspended = new(5_006);
    public static readonly ResultCode AuthAccountNotActivated = new(5_007);
    public static readonly ResultCode AuthBusiness = new(5_100);
    public static readonly ResultCode AuthCustom = new(5_200);

    // File system
    public static readonly ResultCode File = new(6_000);
    public static readonly ResultCode FileNotFound = new(6_001);
    public static readonly ResultCode FileNotEmpty = new(6_002);
    public static readonly ResultCode FileAlreadyExists = new(6_003);
    public static readonly ResultCode FileFileTooLarge = new(6_004);
    public static readonly ResultCode FileFileTooSmall = new(6_005);
    public static readonly ResultCode FileInvalidFormat = new(6_006);
    public static readonly ResultCode FileCorrupted = new(6_007);
    public static readonly ResultCode FileLocked = new(6_008);
    public static readonly ResultCode FileReadOnly = new(6_009);
    public static readonly ResultCode FileInUse = new(6_010);
    public static readonly ResultCode FileAccessDenied = new(6_011);
    public static readonly ResultCode FileInvalidName = new(6_012);
    public static readonly ResultCode FileDirectoryNotFound = new(6_013);
    public static readonly ResultCode FileDirectoryNotEmpty = new(6_014);
    public static readonly ResultCode FileBusiness = new(6_100);
    public static readonly ResultCode FileCustom = new(6_200);

    // Business
    public static readonly ResultCode Business = new(7_000);
    public static readonly ResultCode BusinessStateTransition = new(7_001);
    public static readonly ResultCode BusinessEntityInUse = new(7_002);
    public static readonly ResultCode BusinessKeyDuplicate = new(7_003);
    public static readonly ResultCode BusinessConditionNotMet = new(7_004);
    public static readonly ResultCode BusinessOperationAlreadyCompleted = new(7_005);
    public static readonly ResultCode BusinessInsufficientFunds = new(7_006);
    public static readonly ResultCode BusinessDeadlineExceeded = new(7_007);
    public static readonly ResultCode BusinessCircularReference = new(7_008);
    public static readonly ResultCode BusinessChildEntityLimitExceeded = new(7_009);
    public static readonly ResultCode BusinessDependencyMissing = new(7_010);
    public static readonly ResultCode BusinessLifecyclePhaseViolation = new(7_011);
    public static readonly ResultCode BusinessTemporalConflict = new(7_012);
    public static readonly ResultCode BusinessInvariantViolation = new(7_013);
    public static readonly ResultCode BusinessNotAllowed = new(7_014);
    public static readonly ResultCode BusinessReversalNotAllowed = new(7_015);
    public static readonly ResultCode BusinessBusiness = new(7_100);
    public static readonly ResultCode BusinessCustom = new(7_200);
}