namespace FPTU_ProposalGuard.API.Payloads;

public class APIRoute
{
    private const string Base = "api";
    
    /// <summary>
    /// Authentication endpoints
    /// </summary>
    public static class Authentication
    {
        // [GET]
        public const string ForgotPassword = Base + "/auth/forgot-password";
        public const string CurrentUser = Base + "/auth/current-user";
        public const string GetMfaBackupAsync = Base + "/auth/mfa-backup";
        // [POST]
        public const string SignIn = Base + "/auth/sign-in";
        public const string SignInWithPassword = Base + "/auth/sign-in/password-method";
        public const string SignInWithOtp = Base + "/auth/sign-in/otp-method";
        public const string SignInWithGoogle = Base + "/auth/sign-in-google";
        public const string RefreshToken = Base + "/auth/refresh-token";
        public const string ResendOtp = Base + "/auth/resend-otp";
        public const string ChangePasswordOtpVerification = Base + "/auth/change-password/verify-otp";
        public const string EnableMfa = Base + "/auth/enable-mfa";
        public const string ValidateMfa = Base + "/auth/validate-mfa";
        public const string ValidateBackupCode = Base + "/auth/validate-mfa-backup";
        public const string RegenerateBackupCode = Base + "/auth/regenerate-mfa-backup";
        public const string RegenerateBackupCodeConfirm = Base + "/auth/regenerate-mfa-backup/confirm";
        // [PATCH]
        public const string ConfirmRegistration = Base + "/auth/sign-up/confirm";
        public const string ChangePassword = Base + "/auth/change-password";
        public const string ChangePasswordAsEmployee = Base + "/auth/employee/change-password";
        // [PUT]
        public const string UpdateProfile = Base + "/auth/profile";
    }

    #region Notification endpoints

    public static class Notification
    {
        // [GET]
        public const string GetAll = Base + "/notifications";
        // [POST]
        public const string Create = Base + "/notifications";
        // [PUT]
        // [PATCH]
        // [DELETE]
    }
    #endregion
}