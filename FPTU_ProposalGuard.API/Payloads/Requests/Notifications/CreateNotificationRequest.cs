using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.API.Payloads.Requests.Notifications;

public class CreateNotificationRequest
{
    public int? RecipientId { get; set; }

    public string? Title { get; set; }

    public string Message { get; set; } = null!;

    public NotificationType Type { get; set; }
}