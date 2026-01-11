namespace Tawtheef.Domain.Constants;

public static class ErrorsCodes
{
    // ============================================================
    // CORE / SYSTEM
    // ============================================================
    #region Core

    public const string UnauthorizedAction = "UNAUTHORIZED_ACTION";
    public const string InvalidRecaptcha = "INVALID_RECAPTCHA";
    public const string InvalidDeviceId = "INVALID_DEVICE_ID";
    public const string ConcurrencyFailure = "CONCURRENCY_FAILURE";
    public const string UnExpectedError = "UN_EXPECTED_ERROR";

    #endregion

    // ============================================================
    // CONFIGURATION
    // ============================================================
    #region Configuration

    public const string SiteUrlNotConfigured = "SITE_URL_NOT_CONFIGURED";
    public const string ConfigMissing = "CONFIG_MISSING";
    public const string BlobSignKeyConfigMissing = "BLOB_SIGN_KEY_CONFIG_MISSING";

    #endregion

    // ============================================================
    // AUTHENTICATION & LOGIN
    // ============================================================
    #region Authentication

    public const string LoginFailed = "LOGIN_FAILED";
    public const string InvalidCredentials = "INVALID_CREDENTIALS_PROVICED";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string ProfileNotFound = "PROFILE_NOT_FOUND";
    public const string EmailNotVerified = "EMAIL_NOT_VERIFIED";
    public const string AccountStatusNotAllowedForLogin = "ACCOUNT_STATUS_NOT_ALLOWED_FOR_LOGIN";
    public const string SessionExpired = "SESSION_EXPIRED";
    public const string QatariPeopleNotAllowedLoginBeforeRegisterOnKawader = "QATARI_PEOPLE_NOT_ALLOWED_LOGIN_BEFORE_REGISTER_ON_KAWADER";

    #endregion

    // ============================================================
    // TOKEN / SESSION SECURITY
    // ============================================================
    #region Tokens

    public const string InvalidToken = "INVALID_TOKEN";
    public const string InvalidTokenType = "INVALID_TOKEN_TYPE";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string InvalidAlgorithm = "INVALID_ALGORITHM";

    public const string AccessTokenRequired = "ACCESS_TOKEN_REQUIRED";
    public const string RefreshTokenRequired = "REFRESH_TOKEN_REQUIRED";
    public const string InvalidAccessToken = "INVALID_ACCESS_TOKEN";
    public const string InvalidUserIdentifier = "INVALID_USER_IDENTIFIER";
    public const string RefreshTokenNotFound = "REFRESH_TOKEN_NOT_FOUND";
    public const string InactiveRefreshToken = "INACTIVE_REFRESH_TOKEN";
    public const string SessionRevoked = "SESSION_REVOKED";

    #endregion

    // ============================================================
    // OTP & VERIFICATION
    // ============================================================
    #region Verification

    public const string VerificationCodeExpired = "VERIFICATION_CODE_EXPIRED";
    public const string InvalidVerificationCode = "INVALID_VERIFICATION_CODE";
    public const string InvalidTokenOrEmail = "INVALID_TOKEN_OR_EMAIL";
    public const string InvalidCode = "INVALID_CODE";
    public const string TooManyAttempts = "TOO_MANY_ATTEMPTS";
    public const string ResendCooldownActive = "RESEND_COOLDOWN_ACTIVE";
    public const string SendOtpLimitReached = "SEND_OTP_LIMIT_REACHED";
    public const string PhoneVerificationRateLimited = "PHONE_VERIFICATION_RATE_LIMITED";

    public const string EmailAlreadyVerified = "EMAIL_ALREADY_VERIFIED";
    public const string UserPhoneRequired = "USER_PHONE_REQUIRED";
    public const string ShouldVerifiyQatarPhoneNumberBeforeAssignIt = "SHOULD_VERIFIY_QATAR_PHONE_NUMBER_BEFORE_ASSIGN_IT";

    #endregion

    // ============================================================
    // EXTERNAL AUTH
    // ============================================================
    #region ExternalAuthentication

    public static string ExternalLoginError(string error) => $"EXTERNAL_LOGIN_ERROR: {error}";

    public const string ExternalLoginInfoNotFound = "EXTERNAL_LOGIN_INFO_NOT_FOUND";
    public const string ExternalLoginEmailNotFound = "EXTERNAL_LOGIN_EMAIL_NOT_FOUND";
    public const string ExternalLoginUserNotFound = "EXTERNAL_LOGIN_USER_NOT_FOUND";
    public const string ExternalLoginProviderRequired = "EXTERNAL_LOGIN_PROVIDER_REQUIRED";
    public const string ExternalLoginAlreadyLinked = "EXTERNAL_LOGIN_USER_ALREADY_LINKED";
    public const string ExternalLoginProviderAlreadyLinked = "EXTERNAL_LOGIN_PROVIDER_ALREADY_LINKED";
    public const string ExternalLoginUserTypeNotAllowed = "EXTERNAL_LOGIN_USER_TYPE_NOT_ALLOWED";
    public const string ExternalLoginNotLinkedOfficeUser = "EXTERNAL_LOGIN_NOT_LINKED_OFFICE_USER";
    public const string ExternalLoginOfficeUserInvalidType = "EXTERNAL_LOGIN_OFFICE_USER_INVALID_TYPE";
    public const string ExternalLoginOfficeUserNotLinkedToOffice = "EXTERNAL_LOGIN_OFFICE_USER_NOT_LINKED_TO_OFFICE";
    public const string ExternalLoginOfficeUserOfficeDeletedOrNotFound = "EXTERNAL_LOGIN_OFFICE_USER_OFFICE_DELETED_OR_NOT_FOUND";
    public const string ExternalLoginEmailDomainNotAllowed = "EXTERNAL_LOGIN_EMAIL_DOMAIN_NOT_ALLOWED";

    #endregion

    // ============================================================
    // QATAR SERVICES
    // ============================================================
    #region Qatar

    public const string QatarPassQidNotAllowed = "QATAR_PASS_QID_NOT_ALLOWED";
    public const string QatarResidentInvalidQid = "QATAR_RESIDENT_INVALID_QID";
    public const string QatarResidentPhoneInvalid = "QATAR_RESIDENT_PHONE_INVALID";
    public const string QatarResidentPhoneMismatch = "QATAR_RESIDENT_PHONE_MISMATCH";
    public const string QatarResidentVerificationFailed = "QATAR_RESIDENT_VERIFICATION_FAILED";

    #endregion

    // ============================================================
    // USER REGISTRATION
    // ============================================================
    #region Registration

    public const string EmailRequired = "EMAIL_REQUIRED";
    public const string UserNameRequired = "USER_NAME_REQUIRED";
    public const string PhoneNumberRequired = "PHONE_NUMBER_REQUIRED";
    public const string EmailAlreadyInUse = "EMAIL_ALREADY_IN_USE";
    public const string PhoneAlreadyInUse = "PHONE_ALREADY_IN_USE";
    public const string EmailIsAlreadyTaken = "EMAIL_IS_ALREADY_TAKEN";

    #endregion

    // ============================================================
    // PASSWORD
    // ============================================================
    #region Password

    public const string InvalidPassword = "INVALID_PASSWORD";
    public const string IncorrectOldPassword = "INCORRECT_OLD_PASSWORD";
    public const string PasswordChangeFailed = "PASSWORD_CHANGE_FAILED";
    public const string PasswordResetFailed = "PASSWORD_RESET_FAILED";

    #endregion

    // ============================================================
    // USER PROFILE CORE
    // ============================================================
    #region UserProfile

    public const string UserProfileNotFound = "USER_PROFILE_NOT_FOUND";
    public const string UserProfileNotCompleted = "USER_PROFILE_NOT_COMPLETED";
    public const string ProfileAlreadyApproved = "PROFILE_ALREADY_APPROVED";
    public const string ProfileLockedUnderReview = "PROFILE_LOCKED_UNDER_REVIEW";
    public const string ProfileNotUnderReview = "PROFILE_NOT_UNDER_REVIEW";
    public const string ProfileNotReadyForReview = "PROFILE_NOT_READY_FOR_REVIEW";
    public const string NotSubmitted = "NOT_SUBMITTED";

    public const string MandatoryFieldsIncomplete = "MANDATORY_FIELDS_INCOMPLETE";
    public const string NotesRequiredForCorrection = "NOTES_REQUIRED_FOR_CORRECTION";
    public const string NeedsCorrectionTargetsRequired = "NEEDS_CORRECTION_TARGETS_REQUIRED";

    #endregion

    // ============================================================
    // ATTACHMENTS & FILES
    // ============================================================
    #region Attachments

    public const string UploadFailed = "UPLOAD_FAILED";
    public const string EmptyFile = "EMPTY_FILE";
    public const string InvalidAttachmentType = "INVALID_ATTACHMENT_TYPE";
    public const string InvalidAttachmentSize = "INVALID_ATTACHMENT_SIZE";
    public const string InvalidAttachmentId = "INVALID_ATTACHMENT_ID";
    public const string InvalidAttachmentFile = "INVALID_ATTACHMENT_FILE";
    public const string InvalidAttachmentFileIndex = "INVALID_ATTACHMENT_FILE_INDEX";
    public const string AttachmentNotFound = "ATTACHMENT_NOT_FOUND";
    public const string AttachmentNotEditableInRevision = "ATTACHMENT_NOT_EDITABLE_IN_REVISION";
    public const string DuplicateAttachmentResource = "DUPLICATE_ATTACHMENT_RESOURCE";

    #endregion

    // ============================================================
    // STORAGE
    // ============================================================
    #region Storage

    public const string NotPublicResource = "NOT_PUBLIC_RESOURCE";
    public const string InvalidBlobKey = "INVALID_BLOB_KEY";
    public const string IoError = "IO_ERROR";
    public const string AccessDenied = "ACCESS_DENIED";
    public const string Cancelled = "CANCELLED";
    public const string FileNotFound = "FILE_NOT_FOUND";
    public const string UrlFileExpired = "URL_FILE_EXPIRED";
    public const string OnlyPrivateBlobKeysAllowed = "ONLY_PRIVATE_BLOB_KEYS_ALLOWED";

    #endregion

    // ============================================================
    // DISTRIBUTION
    // ============================================================
    #region Distribution

    public const string DistributionEmployeeNotActive = "DISTRIBUTION_EMPLOYEE_NOT_ACTIVE";
    public const string DistributionProfilesNotFound = "DISTRIBUTION_PROFILES_NOT_FOUND";
    public const string DistributionFinalStatusNotAllowed = "DISTRIBUTION_FINAL_STATUS_NOT_ALLOWED";
    public const string DistributionStatusNotAssignable = "DISTRIBUTION_STATUS_NOT_ASSIGNABLE";
    public const string DistributionNoEligibleEmployees = "DISTRIBUTION_NO_ELIGIBLE_EMPLOYEES";
    public const string DistributionNoAssignableProfiles = "DISTRIBUTION_NO_ASSIGNABLE_PROFILES";
    public const string DistributionModeRequired = "DISTRIBUTION_MODE_REQUIRED";
    public const string DistributionPerEmployeeLimitReached = "DISTRIBUTION_PER_EMPLOYEE_LIMIT_REACHED";
    public const string ProfileNotAssignable = "PROFILE_NOT_ASSIGNABLE";

    #endregion
}
