using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.Application.Dtos.Notifications;

public class NotificationDto
{
    public int NotificationId { get; set; }

    public Guid? RecipientId { get; set; }

    public string? Title { get; set; }

    public string Message { get; set; } = null!;

    public NotificationType Type { get; set; }

    public bool IsPublic { get; set; }
    
    public DateTime CreateDate { get; set; }

    public Guid CreatedById { get; set; }

    public UserDto CreatedBy { get; set; } = null!;

    public ICollection<NotificationRecipientDto> NotificationRecipients { get; set; } = new List<NotificationRecipientDto>();
}