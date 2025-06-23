using FPTU_ProposalGuard.Application.Dtos.Users;

namespace FPTU_ProposalGuard.Application.Dtos.Notifications;

public class NotificationRecipientDto
{
    public int NotificationRecipientId { get; set; }

    public int NotificationId { get; set; }

    public Guid RecipientId { get; set; }

    public bool IsRead { get; set; }

    public UserDto Recipient { get; set; } = null!;
}