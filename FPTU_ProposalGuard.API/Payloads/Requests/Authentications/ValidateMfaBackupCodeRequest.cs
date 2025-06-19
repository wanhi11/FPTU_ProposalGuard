namespace FPTU_ProposalGuard.API.Payloads.Requests.Authentications;

public class ValidateMfaBackupCodeRequest
{
    public string Email { get; set; } = null!;
    public string BackupCode { get; set; } = null!;
}