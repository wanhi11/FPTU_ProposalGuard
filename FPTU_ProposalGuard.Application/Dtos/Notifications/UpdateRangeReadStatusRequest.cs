namespace FPTU_ProposalGuard.Application.Dtos.Notifications;

public class UpdateRangeReadStatusRequest
{
    public List<int> NotificationIds { get; set; } = new();
}