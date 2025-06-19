namespace FPTU_ProposalGuard.Application.Dtos;

public class EmailAttachmentDto
{
    public string FileName { get; set; } = null!;
    public byte[] FileBytes { get; set; } = null!;
    public string ContentType { get; set; } = null!; // Using MediaTypeNames
}