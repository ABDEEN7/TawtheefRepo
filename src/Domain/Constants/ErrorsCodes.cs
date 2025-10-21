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
    public const string EmailNotVerified = "EMAIL_NOT_VERIFIED";
    public const string YourAccountRequiresAdminApproval = "YOUR_ACCOUNT_REQUIRES_ADMIN_APPROVAL";
    public const string SocialAccountNotLinked = "SOCIAL_ACCOUNT_NOT_LINKED";
    public const string UnlinkSocialAccountFailed = "UNLINK_SOCIAL_ACCOUNT_FAILED";
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
    #endregion

    #region Verification Errors
    public const string VerificationCodeExpired = "VERIFICATION_CODE_EXPIRED";
    public const string InvalidTokenOrEmail = "INVALID_TOKEN_OR_EMAIL";
    public const string TooManyAttempts = "TOO_MANY_ATTEMPTS";
    public const string InvalidVerificationCode = "INVALID_VERIFICATION_CODE";
    public const string ResendCooldownActive = "RESEND_COOLDOWN_ACTIVE";
    public const string EmailAlreadyVerified = "EMAIL_ALREADY_VERIFIED";
    public const string UserHasNotEmail = "USER_HAS_NOT_EMAIL";
    #endregion



    #region Update Profile Errors
    public const string InvalidGender = "INVALID_GENDER";
    public const string InvalidName = "INVALID_NAME";
    public const string InvalidPassword = "INVALID_PASSWORD";
    public const string IncorrectOldPassword = "INCORRECT_OLD_PASSWORD";
    public const string PasswordChangeFailed = "PASSWORD_CHANGE_FAILED";
    #endregion

    #region External Authentication Errors
    public static string ExternalLoginError(string error) => $"EXTERNAL_LOGIN_ERROR: {error}";
    public const string ExternalLoginInfoNotFound = "EXTERNAL_LOGIN_INFO_NOT_FOUND";
    public const string ExternalLoginEmailNotFound = "EXTERNAL_LOGIN_EMAIL_NOT_FOUND";
    public const string ExternalLoginUserNotFound = "EXTERNAL_LOGIN_USER_NOT_FOUND";
    public const string ExternalLoginProviderRequired = "EXTERNAL_LOGIN_PROVIDER_REQUIRED";
    public const string ExternalLoginAlreadyLinked = "EXTERNAL_LOGIN_USER_ALREADY_LINKED";
    public const string ExternalLoginProviderAlreadyLinked = "EXTERNAL_LOGIN_PROVIDER_ALREADY_LINKED";
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
    #endregion
}
