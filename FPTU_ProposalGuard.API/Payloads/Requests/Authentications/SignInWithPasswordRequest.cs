namespace FPTU_ProposalGuard.API.Payloads.Requests.Authentications;

public class SignInWithPasswordRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}