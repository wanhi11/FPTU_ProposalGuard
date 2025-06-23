using System.Text.Json.Serialization;

namespace FPTU_ProposalGuard.Domain.Entities;

public class NotificationRecipient
{
    public int NotificationRecipientId { get; set; }

    public int NotificationId { get; set; }

    public Guid RecipientId { get; set; }

    public bool IsRead { get; set; }

    [JsonIgnore]
    public Notification Notification { get; set; } = null!;

    [JsonIgnore]
    public User Recipient { get; set; } = null!;
}