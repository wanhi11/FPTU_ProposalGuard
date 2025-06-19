namespace FPTU_ProposalGuard.Application.Dtos.Authentications;

public class AuthenticateResultDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ValidTo { get; set; }
}