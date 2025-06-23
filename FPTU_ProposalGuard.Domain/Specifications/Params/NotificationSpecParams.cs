using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.Domain.Specifications.Params;

public class NotificationSpecParams : BaseSpecParams
{
    public string? Email { get; set; }
    public string? CreatedBy { get; set; } = null!;
    public bool? isPublic { get; set; }
    public NotificationType? NotificationType { get; set; }
    public DateTime?[]? CreateDateRange { get; set; } 
}