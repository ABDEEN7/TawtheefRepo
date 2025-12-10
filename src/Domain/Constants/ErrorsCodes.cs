using Microsoft.AspNetCore.Identity;

namespace Tawtheef.Domain.Constants;
public class ErrorsCodes
{
    #region Common Errors
    public const string UnauthorizedAction = "UNAUTHORIZED_ACTION";
    public const string InvalidRecaptcha = "INVALID_RECAPTCHA";
    public const string InvalidDeviceId = "INVALID_DEVICE_ID";
    public const string EmailRequired = "EMAIL_REQUIRED";
    public const string UserNameRequired = "USER_NAME_REQUIRED";
    public const string PhoneNumberRequired = "PHONE_NUMBER_REQUIRED";
    public const string ConcurrencyFailure = "CONCURRENCY_FAILURE";
    public const string LoginFailed = "LOGIN_FAILED";
    #endregion

    #region Validation Token Errors
    public const string InvalidToken = "INVALID_TOKEN";
    public const string InvalidTokenType = "INVALID_TOKEN_TYPE";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string InvalidAlgorithm = "INVALID_ALGORITHM";
    #endregion

    #region Application Configuration Errors
    public const string SiteUrlNotConfigured = "SITE_URL_NOT_CONFIGURED";
    #endregion

    #region Login Errors
    public const string InvalidCredentials = "INVALID_CREDENTIALS_PROVICED";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string SessionExpired = "SESSION_EXPIRED";
    public const string EmailNotVerified = "EMAIL_NOT_VERIFIED";
    public const string AccountStatusNotAllowedForLogin = "ACCOUNT_STATUS_NOT_ALLOWED_FOR_LOGIN";
    public const string YourAccountRequiresAdminApproval = "YOUR_ACCOUNT_REQUIRES_ADMIN_APPROVAL";
    public const string SocialAccountNotLinked = "SOCIAL_ACCOUNT_NOT_LINKED";
    public const string UnlinkSocialAccountFailed = "UNLINK_SOCIAL_ACCOUNT_FAILED";
    public const string ExternalLoginMissingTokens = "EXTERNAL_LOGIN_MISSING_TOKENS";
    public static string ExternalLoginCreateUserFailed(IEnumerable<IdentityError> error) =>
        $"EXTERNAL_LOGIN_CREATE_USER_FAILED: {error}";
    public static string ExternalLoginLinkFailed(IEnumerable<IdentityError> error) =>
        $"EXTERNAL_LOGIN_LINK_FAILED: {error}";
    #endregion

    #region Registration Errors
    public const string EmailIsAlreadyTaken = "EMAIL_IS_ALREADY_TAKEN";
    #endregion

    #region Refresh Token Errors
    public const string AccessTokenRequired = "ACCESS_TOKEN_REQUIRED";
    public const string RefreshTokenRequired = "REFRESH_TOKEN_REQUIRED";
    public const string InvalidAccessToken = "INVALID_ACCESS_TOKEN";
    public const string InvalidUserIdentifier = "INVALID_USER_IDENTIFIER";
    public const string RefreshTokenNotFound = "REFRESH_TOKEN_NOT_FOUND";
    public const string InactiveRefreshToken = "INACTIVE_REFRESH_TOKEN";
    public const string SessionRevoked = "SESSION_REVOKED";
    #endregion

    #region Verification Errors
    public const string VerificationCodeExpired = "VERIFICATION_CODE_EXPIRED";
    public const string InvalidTokenOrEmail = "INVALID_TOKEN_OR_EMAIL";
    public const string TooManyAttempts = "TOO_MANY_ATTEMPTS";
    public const string InvalidVerificationCode = "INVALID_VERIFICATION_CODE";
    public const string ResendCooldownActive = "RESEND_COOLDOWN_ACTIVE";
    public const string EmailAlreadyVerified = "EMAIL_ALREADY_VERIFIED";
    public const string SendOtpLimitReached = "SEND_OTP_LIMIT_REACHED";
    public const string UserPhoneRequired = "USER_PHONE_REQUIRED";
    public const string InvalidCode = "INVALID_CODE";
    public const string PhoneVerificationRateLimited = "PHONE_VERIFICATION_RATE_LIMITED";
    #endregion

    #region Update Profile Errors
    public const string UserProfileNotCompleted = "USER_PROFILE_NOT_COMPLETED";
    public const string UserProfileNotFound = "USER_PROFILE_NOT_FOUND";
    public const string InvalidGender = "INVALID_GENDER";
    public const string InvalidName = "INVALID_NAME";
    public const string InvalidPassword = "INVALID_PASSWORD";
    public const string IncorrectOldPassword = "INCORRECT_OLD_PASSWORD";
    public const string PasswordChangeFailed = "PASSWORD_CHANGE_FAILED";
    public const string DegreeNotFound = "DEGREE_NOT_FOUND";
    public const string ReviewItemNotFound = "REVIEW_ITEM_NOT_FOUND";
    public const string ExperienceNotFound = "EXPERIENCE_NOT_FOUND";
    public const string SkillNotFound = "SKILL_NOT_FOUND";
    public const string LanguageProfileNotFound = "LANGUAGE_PROFILE_NOT_FOUND";
    public const string TrainingNotFound = "TRAINING_NOT_FOUND";
    public const string AchievementNotFound = "ACHIEVEMENT_NOT_FOUND";
    public const string SectionMustBeApprovedFirst = "SECTION_MUST_BE_APPROVED_FIRST";
    public const string SectionHasUnapprovedAttachments = "SECTION_HAS_UNAPPROVED_ATTACHMENTS";
    public const string NotesRequiredForCorrection = "NOTES_REQUIRED_FOR_CORRECTION";
    public const string MandatoryFieldsIncomplete = "MANDATORY_FIELDS_INCOMPLETE";
    public const string UnapprovedItemsExist = "UNAPPROVED_ITEMS_EXIST";
    public const string RejectionDocumentRequired = "REJECTION_DOCUMENT_REQUIRED";
    public const string ExceptionalFileRequired = "EXCEPTIONAL_FILE_REQUIRED";
    public const string NeedsCorrectionTargetsRequired = "NEEDS_CORRECTION_TARGETS_REQUIRED";
    public const string InvalidDegreeUniversityId = "INVALID_DEGREE_UNIVERSITY_ID";
    public const string InvalidDegreeMajorId = "INVALID_DEGREE_MAJOR_ID";
    public const string InvalidGpa = "INVALID_GPA";
    public const string InvalidDegreeFile = "INVALID_DEGREE_FILE";
    public const string InvalidDegreeId = "INVALID_DEGREE_ID";
    public const string InvalidDegreeCountryId = "INVALID_DEGREE_COUNTRY_ID";
    public const string InvalidDegreeGradeId = "INVALID_DEGREE_GRADE_ID";
    public const string InvalidGradYear = "INVALID_GRAD_YEAR";
    public const string InvalidDegreeStudyTypeId = "INVALID_DEGREE_STUDY_TYPE_ID";
    public const string InvalidDegreeSubMajorId = "INVALID_DEGREE_SUB_MAJOR_ID";
    public const string DegreeLinkedToExperience = "DEGREE_LINKED_TO_EXPERIENCE";
    public const string InvalidExperienceQualification = "INVALID_EXPERIENCE_QUALIFICATION";
    public const string ExperienceBeforeGraduation = "EXPERIENCE_BEFORE_GRADUATION";
    #endregion

    #region External Authentication Errors
    public static string ExternalLoginError(string error) => $"EXTERNAL_LOGIN_ERROR: {error}";
    public const string ExternalLoginInfoNotFound = "EXTERNAL_LOGIN_INFO_NOT_FOUND";
    public const string ExternalLoginEmailNotFound = "EXTERNAL_LOGIN_EMAIL_NOT_FOUND";
    public const string ExternalLoginUserNotFound = "EXTERNAL_LOGIN_USER_NOT_FOUND";
    public const string ExternalLoginProviderRequired = "EXTERNAL_LOGIN_PROVIDER_REQUIRED";
    public const string ExternalLoginAlreadyLinked = "EXTERNAL_LOGIN_USER_ALREADY_LINKED";
    public const string ExternalLoginProviderAlreadyLinked = "EXTERNAL_LOGIN_PROVIDER_ALREADY_LINKED";
    public const string ExternalLoginProviderNotSupported = "";
    public const string ExternalLoginInvalidToken = "";
    public const string ExternalLoginMissingProviderKey = "";
    public const string InvalidRequest = "";
    #endregion

    #region Reset Password Errors
    public const string PasswordResetFailed = "PASSWORD_RESET_FAILED";
    #endregion

    #region Storage Errors
    public const string ConfigMissing = "CONFIG_MISSING";
    public const string NotPublicResource = "NOT_PUBLIC_RESOURCE";
    public const string InvalidBlobKey = "INVALID_BLOB_KEY";
    public const string IoError = "IO_ERROR";
    public const string AccessDenied = "ACCESS_DENIED";
    public const string Cancelled = "CANCELLED";
    public const string EmptyFile = "EMPTY_FILE";
    #endregion

    #region Resources Error
    public const string BlobSignKeyConfigMissing = "BLOB_SIGN_KEY_CONFIG_MISSING";
    public const string OnlyPrivateBlobKeysAllowed = "ONLY_PRIVATE_BLOB_KEYS_ALLOWED";
    public const string UnExpectedError = "UN_EXPECTED_ERROR";
    public const string FileNotFound = "FILE_NOT_FOUND";
    public const string UrlFileExpired = "URL_FILE_EXPIRED";
    #endregion

    #region User Profile
    public const string InvalidDegreesJson = "INVALID_DEGREES_JSON";
    public const string InvalidDegreesCount = "INVALID_DEGREES_COUNT";
    public const string InvalidExperiencesJson = "INVALID_EXPERIENCES_JSON";
    public const string InvalidTrainingCoursesJson = "INVALID_TRAINING_COURSES_JSON";
    public const string InvalidAchievementsJson = "INVALID_ACHIEVEMENTS_JSON";
    public const string InvalidExperienceFileIndex = "INVALID_EXPERIENCE_FILE_INDEX";
    public const string InvalidTrainingCourseFileIndex = "INVALID_TRAINING_COURSE_FILE_INDEX";
    public const string InvalidAchievementFileIndex = "INVALID_ACHIEVEMENT_FILE_INDEX";
    public const string InvalidExperienceFile = "INVALID_EXPERIENCE_FILE";
    public const string InvalidTrainingCourseFile = "INVALID_TRAINING_COURSE_FILE";
    public const string InvalidAchievementFile = "INVALID_ACHIEVEMENT_FILE";
    public const string ExperienceDescriptionTooLong = "EXPERIENCE_DESCRIPTION_TOO_LONG";
    public const string TrainingDescriptionTooLong = "TRAINING_DESCRIPTION_TOO_LONG";
    public const string AchievementDescriptionTooLong = "ACHIEVEMENT_DESCRIPTION_TOO_LONG";
    public const string ExperienceFileTooLarge = "EXPERIENCE_FILE_TOO_LARGE";
    public const string TrainingCourseFileTooLarge = "TRAINING_COURSE_FILE_TOO_LARGE";
    public const string AchievementFileTooLarge = "ACHIEVEMENT_FILE_TOO_LARGE";
    public const string InvalidAttachmentsJson = "INVALID_ATTACHMENTS_JSON";
    public const string InvalidAttachmentFileIndex = "INVALID_ATTACHMENT_FILE_INDEX";
    public const string InvalidAttachmentFile = "INVALID_ATTACHMENT_FILE";
    public const string CvFileRequired = "CV_FILE_REQUIRED";
    public const string IdFileRequired = "ID_FILE_REQUIRED";
    public const string MarriageCertificateFileRequired = "MARRIAGE_CERTIFICATE_FILE_REQUIRED";
    public const string BirthCertificateFileRequired = "BIRTH_CERTIFICATE_FILE_REQUIRED";
    public const string SponsorCardRequired = "SPONSOR_CARD_REQUIRED";
    public const string SponsorCardFileRequired = "SPONSOR_CARD_FILE_REQUIRED";
    public const string OfficeRequired = "OFFICE_REQUIRED";
    public const string AddressRequired = "ADDRESS_REQUIRED";
    public const string NationalAddressRequired = "NATIONAL_ADDRESS_REQUIRED";
    public const string NationalAddressDocumentRequired = "NATIONAL_ADDRESS_DOCUMENT_REQUIRED";
    public const string DegreeFileRequired = "DEGREE_FILE_REQUIRED";
    public const string ExperienceRequired = "EXPERIENCE_REQUIRED";
    public const string SkillOrLanguageRequired = "SKILL_OR_LANGUAGE_REQUIRED";
    #endregion

    #region Attachments

    public const string InvalidAttachmentType = "INVALID_ATTACHMENT_TYPE";
    public const string InvalidAttachmentSize = "INVALID_ATTACHMENT_SIZE";
    public const string AttachmentNotFound = "ATTACHMENT_NOT_FOUND";

    public const string PreviousProfileStepIncomplete = "PREVIOUS_PROFILE_STEP_INCOMPLETE";
    public const string CandidateTypeChangeNotAllowed = "CANDIDATE_TYPE_CHANGE_NOT_ALLOWED";
    public const string SponsorNotAllowed = "SPONSOR_NOT_ALLOWED";
    public const string NationalAddressNotAllowed = "NATIONAL_ADDRESS_NOT_ALLOWED";

    #endregion

    #region Profile Distribution
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

