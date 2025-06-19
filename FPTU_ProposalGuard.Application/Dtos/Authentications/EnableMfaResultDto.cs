namespace FPTU_ProposalGuard.Application.Dtos.Authentications;

public class EnableMfaResultDto
{
    public string QrCodeImage { get; set; } = null!;
    public IEnumerable<string> BackupCodes { get; set; } = null!;
}