using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.Domain.Entities;

public class Notification
{
    public int NotificationId { get; set; }

    public Guid? RecipientId { get; set; }

    public string? Title { get; set; }

    public string Message { get; set; } = null!;

    public NotificationType Type { get; set; }

    public bool IsPublic { get; set; }
    
    public DateTime CreateDate { get; set; }

    public Guid CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;

    public ICollection<NotificationRecipient> NotificationRecipients { get; set; } = new List<NotificationRecipient>();
}
