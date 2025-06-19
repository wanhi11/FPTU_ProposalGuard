namespace FPTU_ProposalGuard.API.Payloads.Requests.Authentications;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
}