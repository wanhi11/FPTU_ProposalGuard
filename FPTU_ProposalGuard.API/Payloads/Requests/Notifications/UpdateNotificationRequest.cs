using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.API.Payloads.Requests.Notifications;

public class UpdateNotificationRequest
{
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType NotificationType { get; set; } 
}