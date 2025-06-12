using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.Domain.Specifications.Params;

public class NotificationSpecParams : BaseSpecParams
{
    public Guid? RecipientId { get; set; }
    public bool? IsRead { get; set; }
    public NotificationType? Type { get; set; }
    public DateTime?[]? CreateDateRange { get; set; }
}