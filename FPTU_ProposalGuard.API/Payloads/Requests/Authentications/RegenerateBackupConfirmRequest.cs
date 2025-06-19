namespace FPTU_ProposalGuard.API.Payloads.Requests.Authentications;

public class RegenerateBackupConfirmRequest
{
    public string Otp { get; set; } = null!;
    public string Token { get; set; } = null!;
}