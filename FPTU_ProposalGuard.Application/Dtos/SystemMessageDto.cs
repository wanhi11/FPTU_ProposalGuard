namespace FPTU_ProposalGuard.Application.Dtos;

public class SystemMessageDto
{
    public string MsgId { get; set; }
    public string MsgContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}