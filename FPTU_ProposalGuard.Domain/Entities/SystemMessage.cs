using FPTU_ProposalGuard.Domain.Interfaces;

namespace FPTU_ProposalGuard.Domain.Entities;

public class SystemMessage : IAuditableEntity
{
    public string MsgId { get; set; }
    public string MsgContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}