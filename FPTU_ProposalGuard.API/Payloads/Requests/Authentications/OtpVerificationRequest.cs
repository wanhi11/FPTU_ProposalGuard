namespace FPTU_ProposalGuard.API.Payloads.Requests.Authentications;

public class OtpVerificationRequest
{
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
}