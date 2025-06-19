namespace FPTU_ProposalGuard.Application.Dtos.Authentications;

public class RecoveryPasswordResultDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
}