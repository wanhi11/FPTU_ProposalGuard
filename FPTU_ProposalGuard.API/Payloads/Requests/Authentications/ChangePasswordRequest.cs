namespace FPTU_ProposalGuard.API.Payloads.Requests.Authentications;

public class ChangePasswordRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Token { get; set; }
}
