namespace Tawtheef.Domain.Constants;

public class ErrorsCodes
{
    // =========================================================
    // COMMON / GENERIC
    // =========================================================
    #region Common Errors
    public const string UnauthorizedAction = "UNAUTHORIZED_ACTION";
    public const string EmailRequired = "EMAIL_REQUIRED";
    public const string NameArRequired = "NAME_AR_REQUIRED";
    public const string NameEnRequired = "NAME_EN_REQUIRED";
    public const string UserNameRequired = "USER_NAME_REQUIRED";
    public const string PhoneNumberRequired = "PHONE_NUMBER_REQUIRED";
    public const string UploadFailed = "UPLOAD_FAILED";
    public const string InvalidFileType = "INVALID_FILE_TYPE";
    public const string ResidenceAddressNotFound = "RESIDENCE_ADDRESS_NOT_FOUND";
    public const string SponsorProfileNotFound = "SPONSOR_PROFILE_NOT_FOUND";
    public const string NotSupported = "NOT_SUPPORTED";
    public const string RequestContainsInvalidOrUnsafeContent = "REQUEST_CONTAINS_INVALID_OR_UNSAFE_CONTENT";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string ItemNotFound = "ITEM_NOT_FOUND";
    #endregion

    // =========================================================
    // TOKEN / CRYPTO
    // =========================================================
    #region Validation Token Errors
    public const string InvalidAlgorithm = "INVALID_ALGORITHM";
    #endregion

    // =========================================================
    // LOGIN / AUTH
    // =========================================================
    #region Login Errors
    public const string ProfileNotFound = "PROFILE_NOT_FOUND";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string EmailAlreadyInUse = "EMAIL_ALREADY_IN_USE";
    public const string PhoneAlreadyInUse = "PHONE_ALREADY_IN_USE";
    public const string AccountStatusNotAllowedForLogin = "ACCOUNT_STATUS_NOT_ALLOWED_FOR_LOGIN";

    public const string ExternalLoginNotLinkedOfficeUser = "EXTERNAL_LOGIN_NOT_LINKED_OFFICE_USER";
    public const string ExternalLoginOfficeUserInvalidType = "EXTERNAL_LOGIN_OFFICE_USER_INVALID_TYPE";
    public const string ExternalLoginOfficeUserNotLinkedToOffice = "EXTERNAL_LOGIN_OFFICE_USER_NOT_LINKED_TO_OFFICE";
    public const string ExternalLoginOfficeUserOfficeDeletedOrNotFound = "EXTERNAL_LOGIN_OFFICE_USER_OFFICE_DELETED_OR_NOT_FOUND";

    public const string QatarPassQidNotAllowed = "QATAR_PASS_QID_NOT_ALLOWED";
    public const string QatarResidentInvalidQid = "QATAR_RESIDENT_INVALID_QID";
    public const string QatarResidentPhoneInvalid = "QATAR_RESIDENT_PHONE_INVALID";
    public const string QatarResidentPhoneMismatch = "QATAR_RESIDENT_PHONE_MISMATCH";
    public const string QatarResidentVerificationFailed = "QATAR_RESIDENT_VERIFICATION_FAILED";
    public const string UserIsOfficer = "USER_IS_OFFICER";
    public const string UserIsApplicant = "USER_IS_APPLICANT";
    #endregion

    // =========================================================
    // ROLES / SECURITY
    // =========================================================
    #region Role Management Errors
    public const string RoleNotFound = "ROLE_NOT_FOUND";
    public const string RoleNameExists = "ROLE_NAME_EXISTS";
    public const string InvalidPermission = "INVALID_PERMISSION";
    public const string SystemRoleModificationNotAllowed = "SYSTEM_ROLE_MODIFICATION_NOT_ALLOWED";
    public const string MultipleSystemRolesNotAllowed = "MULTIPLE_SYSTEM_ROLES_NOT_ALLOWED";
    public const string SystemAdminAssignmentNotAllowed = "SYSTEM_ADMIN_ASSIGNMENT_NOT_ALLOWED";
    public const string SystemAdminBlockNotAllowed = "SYSTEM_ADMIN_BLOCK_NOT_ALLOWED";
    public const string SystemRoleRequired = "SYSTEM_ROLE_REQUIRED";
    public const string SystemRoleChangeNotAllowed = "SYSTEM_ROLE_CHANGE_NOT_ALLOWED";
    #endregion

    // =========================================================
    // SESSION / REFRESH TOKEN
    // =========================================================
    #region Refresh Token Errors
    public const string AccessTokenRequired = "ACCESS_TOKEN_REQUIRED";
    public const string RefreshTokenRequired = "REFRESH_TOKEN_REQUIRED";
    public const string InvalidAccessToken = "INVALID_ACCESS_TOKEN";
    public const string InvalidUserIdentifier = "INVALID_USER_IDENTIFIER";
    public const string RefreshTokenNotFound = "REFRESH_TOKEN_NOT_FOUND";
    public const string InactiveRefreshToken = "INACTIVE_REFRESH_TOKEN";
    public const string SessionRevoked = "SESSION_REVOKED";
    #endregion

    // =========================================================
    // OTP / VERIFICATION
    // =========================================================
    #region Verification Errors
    public const string VerificationCodeExpired = "VERIFICATION_CODE_EXPIRED";
    public const string TooManyAttempts = "TOO_MANY_ATTEMPTS";
    public const string InvalidVerificationCode = "INVALID_VERIFICATION_CODE";
    public const string InvalidCode = "INVALID_CODE";
    public const string SendOtpLimitReached = "SEND_OTP_LIMIT_REACHED";
    public const string UserPhoneRequired = "USER_PHONE_REQUIRED";
    public const string PhoneVerificationRateLimited = "PHONE_VERIFICATION_RATE_LIMITED";
    public const string ShouldVerifyQatarPhoneNumberBeforeAssignIt = "SHOULD_VERIFIY_QATAR_PHONE_NUMBER_BEFORE_ASSIGN_IT";
    #endregion

    // =========================================================
    // PROFILE CORE
    // =========================================================
    #region Update Profile Errors
    public const string UserProfileNotCompleted = "USER_PROFILE_NOT_COMPLETED";
    public const string UserProfileNotFound = "USER_PROFILE_NOT_FOUND";
    public const string ProfileNotUnderReview = "PROFILE_NOT_UNDER_REVIEW";
    public const string ProfileNotReadyForReview = "PROFILE_NOT_READY_FOR_REVIEW";
    public const string ProfileLockedUnderReview = "PROFILE_LOCKED_UNDER_REVIEW";
    public const string NotSubmitted = "NOT_SUBMITTED";

    public const string VerifiedIdentityNotReady = "VERIFIED_IDENTITY_NOT_READY";

    public const string InvalidName = "INVALID_NAME";

    public const string DegreeNotFound = "DEGREE_NOT_FOUND";
    public const string ReviewItemNotFound = "REVIEW_ITEM_NOT_FOUND";
    public const string ExperienceNotFound = "EXPERIENCE_NOT_FOUND";
    public const string LanguageProfileNotFound = "LANGUAGE_PROFILE_NOT_FOUND";
    public const string TrainingNotFound = "TRAINING_NOT_FOUND";
    
    public const string CanNotModifiedApprovedDocument = "CAN_NOT_MODIFIED_APPROVED_DOCUMENT";

    public const string DuplicateNationalNumber = "DUPLICATE_NATIONAL_NUMBER";
    public const string NotesRequiredForCorrection = "NOTES_REQUIRED_FOR_CORRECTION";
    public const string UnapprovedItemsExist = "UNAPPROVED_ITEMS_EXIST";
    public const string SpecializationRelationRequired = "SPECIALIZATION_RELATION_REQUIRED";

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
    public const string NoChangesMade = "NO_CHANGES_MADE";
    
    #endregion

    // =========================================================
    // EXTERNAL AUTH
    // =========================================================
    #region External Authentication Errors
    public static string ExternalLoginError(string error) => $"EXTERNAL_LOGIN_ERROR: {error}";
    public const string ExternalLoginInfoNotFound = "EXTERNAL_LOGIN_INFO_NOT_FOUND";
    public const string ExternalLoginEmailNotFound = "EXTERNAL_LOGIN_EMAIL_NOT_FOUND";
    public const string ExternalLoginUserNotFound = "EXTERNAL_LOGIN_USER_NOT_FOUND";
    public const string ExternalLoginProviderRequired = "EXTERNAL_LOGIN_PROVIDER_REQUIRED";
    public const string ExternalLoginAlreadyLinked = "EXTERNAL_LOGIN_USER_ALREADY_LINKED";
    public const string ExternalLoginEmailDomainNotAllowed = "EXTERNAL_LOGIN_EMAIL_DOMAIN_NOT_ALLOWED";
    public const string ExternalLoginInvalidToken = "EXTERNAL_LOGIN_INVALID_TOKEN";
    public const string ExternalLoginMissingProviderKey = "EXTERNAL_LOGIN_MISSING_PROVIDER_KEY";
    public const string InvalidRequest = "INVALID_REQUEST";
    #endregion

    // =========================================================
    // STORAGE
    // =========================================================
    #region Storage Errors
    public const string ConfigMissing = "CONFIG_MISSING";
    public const string NotPublicResource = "NOT_PUBLIC_RESOURCE";
    public const string InvalidBlobKey = "INVALID_BLOB_KEY";
    public const string IoError = "IO_ERROR";
    public const string AccessDenied = "ACCESS_DENIED";
    public const string NotFound = "NOT_FOUND";
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

    // =========================================================
    // USER PROFILE FILES
    // =========================================================
    #region User Profile
    public const string InvalidDegreesJson = "INVALID_DEGREES_JSON";
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
    public const string DuplicateAttachmentResource = "DUPLICATE_ATTACHMENT_RESOURCE";
    public const string InvalidAttachmentFile = "INVALID_ATTACHMENT_FILE";
    public const string CvFileRequired = "CV_FILE_REQUIRED";
    public const string IdFileRequired = "ID_FILE_REQUIRED";
    public const string MarriageCertificateFileRequired = "MARRIAGE_CERTIFICATE_FILE_REQUIRED";
    public const string BirthCertificateFileRequired = "BIRTH_CERTIFICATE_FILE_REQUIRED";
    public const string SponsorCardRequired = "SPONSOR_CARD_REQUIRED";
    public const string OfficeRequired = "OFFICE_REQUIRED";
    public const string AddressRequired = "ADDRESS_REQUIRED";
    public const string InvalidNationalAddress = "INVALID_NATIONAL_ADDRESS";
    public const string NationalAddressCertificateRequired = "NATIONAL_ADDRESS_CERTIFICATE_REQUIRED";
    public const string NationalAddressRequired = "NATIONAL_ADDRESS_REQUIRED";
    public const string NationalAddressDocumentRequired = "NATIONAL_ADDRESS_DOCUMENT_REQUIRED";
    public const string DegreeFileRequired = "DEGREE_FILE_REQUIRED";
    public const string ExperienceRequired = "EXPERIENCE_REQUIRED";
    public const string SkillOrLanguageRequired = "SKILL_OR_LANGUAGE_REQUIRED";
    public const string UserUpdateFailed = "USER_UPDATE_FAILED";
    #endregion

    // =========================================================
    // ATTACHMENT RULES
    // =========================================================
    #region Attachments
    public const string AttachmentNotFound = "ATTACHMENT_NOT_FOUND";
    public const string AttachmentNotEditableInRevision = "ATTACHMENT_NOT_EDITABLE_IN_REVISION";
    public const string InvalidAttachmentId = "INVALID_ATTACHMENT_ID";
    public const string PreviousProfileStepIncomplete = "PREVIOUS_PROFILE_STEP_INCOMPLETE";
    public const string CandidateTypeChangeNotAllowed = "CANDIDATE_TYPE_CHANGE_NOT_ALLOWED";
    public const string NationalAddressNotAllowed = "NATIONAL_ADDRESS_NOT_ALLOWED";
    #endregion

    // =========================================================
    // DISTRIBUTION
    // =========================================================
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

    // =========================================================
    // JOBS
    // =========================================================
    #region Job Management
    public const string InvitationNotFound = "INVITATION_NOT_FOUND";
    public const string InvitationStatusChangeNotAllowed = "INVITATION_STATUS_CHANGE_NOT_ALLOWED";
    public const string JobRequiredAttachmentMissing = "JOB_REQUIRED_ATTACHMENT_MISSING";
    public const string JobRequiredAttachmentUploadFailed = "JOB_REQUIRED_ATTACHMENT_UPLOAD_FAILED";
    #endregion

    // =========================================================
    // ORGANIZATION
    // =========================================================
    #region Organization Structures
    public const string SectorNotFound = "SECTOR_NOT_FOUND";
    public const string SectorNameAlreadyExists = "SECTOR_NAME_ALREADY_EXISTS";
    public const string ManagementNotFound = "MANAGEMENT_NOT_FOUND";
    public const string ManagementNameAlreadyExists = "MANAGEMENT_NAME_ALREADY_EXISTS";
    public const string DepartmentNotFound = "DEPARTMENT_NOT_FOUND";
    public const string DepartmentNameAlreadyExists = "DEPARTMENT_NAME_ALREADY_EXISTS";
    #endregion

    // =========================================================
    // EXTERNAL SERVICES
    // =========================================================
    #region MOI Service
    public const string MOIDataNotFound = "MOI_DATA_NOT_FOUND";
    public const string MOIFailedRequest = "MOI_FAILED_REQUEST";
    #endregion

    public const string EmployeeDirectoryRequestFailed = "EMPLOYEE_DIRECTORY_REQUEST_FAILED";
    public const string EmployeeDirectoryInvalidPayload = "EMPLOYEE_DIRECTORY_INVALID_PAYLOAD";
    public const string EmployeeDirectoryUserNotFound = "EMPLOYEE_DIRECTORY_USER_NOT_FOUND";
    public const string QatariPeopleNotAllowedLoginBeforeRegisterOnKawader = "QATARI_PEOPLE_NOT_ALLOWED_LOGIN_BEFORE_REGISTER_ON_KAWADER";

    // =========================================================
    // LOOKUPS
    // =========================================================
    #region Countries
    public const string CountryNotFound = "COUNTRY_NOT_FOUND";
    #endregion

    #region Languages
    public const string LanguageNotFound = "LANGUAGE_NOT_FOUND";
    public const string LanguageNameExists = "LANGUAGE_NAME_EXISTS";
    #endregion

    #region Universities
    public const string UniversityNotFound = "UNIVERSITY_NOT_FOUND";
    public const string UniversityNameExists = "UNIVERSITY_NAME_EXISTS";
    public const string CityNotFound = "CITY_NOT_FOUND";
    #endregion

    #region Religions
    public const string ReligionNotFound = "RELIGION_NOT_FOUND";
    public const string ReligionNameExists = "RELIGION_NAME_EXISTS";
    #endregion

    #region Target Entities
    public const string TargetEntityNotFound = "TARGET_ENTITY_NOT_FOUND";
    public const string TargetEntityNameExists = "TARGET_ENTITY_NAME_EXISTS";
    #endregion

    #region Job Titles
    public const string JobTitleNumberExists = "JOB_TITLE_NUMBER_EXISTS";
    public const string JobTitleInUse = "JOB_TITLE_IN_USE";
    #endregion

    #region Offices
    public const string OfficeNotFound = "OFFICE_NOT_FOUND";
    public const string OfficeAdminEmailInvalid = "OFFICE_ADMIN_EMAIL_INVALID";
    public const string OfficeAdminEmailExists = "OFFICE_ADMIN_EMAIL_EXISTS";
    public const string OfficeSupportedCountriesRequired = "OFFICE_SUPPORTED_COUNTRIES_REQUIRED";
    public const string OfficeAdminCreationFailed = "OFFICE_ADMIN_CREATION_FAILED";
    public const string OfficeRoleAssignmentFailed = "OFFICE_ROLE_ASSIGNMENT_FAILED";
    public const string OfficeUserNotFound = "OFFICE_USER_NOT_FOUND";
    public const string OfficeAdminNotFound = "OFFICE_ADMIN_NOT_FOUND";
    public const string OfficeNameRequired = "OFFICE_NAME_REQUIRED";
    public const string OfficeCountryRequired = "OFFICE_COUNTRY_REQUIRED";
    public const string OfficeCountryAlreadyAssigned = "OFFICE_COUNTRY_ALREADY_ASSIGNED";
    public const string OfficeAdminAssignFailed = "OFFICE_ADMIN_ASSIGN_FAILED";
    public const string OfficeAdminBlockNotAllowed = "OFFICE_ADMIN_BLOCK_NOT_ALLOWED";
    public const string OfficeAdminEmailChangeNotAllowed = "OFFICE_ADMIN_EMAIL_CHANGE_NOT_ALLOWED";
    #endregion

    #region Major Managements
    public const string MajorNotFound = "MAJOR_NOT_FOUND";
    public const string MajorAlreadyUsed = "MAJOR_ALREADY_USED";
    public const string ChangingHierarchyTypeNotAllowed = "CHANGING_HIERARCHY_TYPE_NOT_ALLOWED";
    public const string MajorNameAlreadyExists = "MAJOR_NAME_ALREADY_EXISTS";
    public const string MajorSkillLinkAlreadyExists = "MAJOR_SKILL_LINK_ALREADY_EXISTS";
    public const string MajorSkillLinkNotFound = "MAJOR_SKILL_LINK_NOT_FOUND";
    public const string CannotLinkInactiveMajorOrSkill = "CANNOT_LINK_INACTIVE_MAJOR_OR_SKILL";
    public const string CannotActivateMajorSkillLinkBecauseParentMajorIsInactive = "CANNOT_ACTIVATE_MAJOR_SKILL_LINK_BECAUSE_PARENT_MAJOR_IS_INACTIVE";
    public const string CannotActivateMajorSkillLinkBecauseMajorIsInactive = "CANNOT_ACTIVATE_MAJOR_SKILL_LINK_BECAUSE_MAJOR_IS_INACTIVE";
    public const string CannotActivateMajorSkillLinkBecauseSkillIsInactive = "CANNOT_ACTIVATE_MAJOR_SKILL_LINK_BECAUSE_SKILL_IS_INACTIVE";
    #endregion

    #region Skill Managements
    public const string SkillNotFound = "SKILL_NOT_FOUND";
    public const string SkillAlreadyUsed = "SKILL_ALREADY_USED";
    #endregion

    #region Minister Office
    public const string InvalidQidFormat = "INVALID_QID_FORMAT";
    public const string InvalidPhoneFormat = "INVALID_PHONE_FORMAT";
    public const string MoiValidationFailed = "MOI_FAILED_REQUEST";
    public const string KawaderRegistrationRequired = "KAWADER_REGISTRATION_REQUIRED";
    public const string MissingRequiredFields = "MISSING_REQUIRED_FIELDS";
    public const string MinisterOfficeCandidateNotFound = "MINISTER_OFFICE_CANDIDATE_NOT_FOUND";
    public const string QidExpiryDateRequired = "QID_EXPIRY_DATE_REQUIRED";
    #endregion
}
public static class CandidateEligibilityConditionCodes
{
    public const string ProfileApproved = "PROFILE_APPROVED";
    public const string AvailableForRecruitment = "AVAILABLE_FOR_RECRUITMENT";
    public const string TargetEntityMatch = "TARGET_ENTITY_MATCH";
    public const string NoActiveInvitation = "NO_ACTIVE_INVITATION";
    public const string GenderMatch = "GENDER_MATCH";
    public const string BirthDateExists = "BIRTH_DATE_EXISTS";
    public const string AgeWithinRange = "AGE_WITHIN_RANGE";
    public const string QualificationMatch = "QUALIFICATION_MATCH";
    public const string RequiredSkillsMatch = "REQUIRED_SKILLS_MATCH";
}

public static class CandidateEligibilityStatuss
{
    public const string Passed = "PASSED";
    public const string Failed = "FAILED";
    public const string NotApplicable = "NOT_APPLICABLE";
}

public static class CandidateEligibilityValueCodes
{
    public const string Unknown = "UNKNOWN";
    public const string None = "NONE";
    public const string All = "ALL";

    public const string Approved = "APPROVED";
    public const string NotApproved = "NOT_APPROVED";

    public const string Available = "AVAILABLE";
    public const string NotAvailable = "NOT_AVAILABLE";

    public const string NoActiveInvitation = "NO_ACTIVE_INVITATION";
    public const string HasActiveInvitation = "HAS_ACTIVE_INVITATION";

    public const string BirthDateExists = "BIRTH_DATE_EXISTS";
    public const string Missing = "MISSING";
    public const string MissingBirthDate = "MISSING_BIRTH_DATE";
}

public static class CandidateEligibilityErrorCodes
{
    public const string JobNotFound = "JOB_NOT_FOUND";
    public const string CandidateNotFound = "CANDIDATE_NOT_FOUND";
}
