using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(e => e.NotificationId).HasName("PK_Notification_NotificationId");

        builder.ToTable("Notification");

        builder.Property(e => e.NotificationId).HasColumnName("notification_id");
        builder.Property(e => e.CreateDate)
            .HasColumnType("datetime")
            .HasColumnName("create_date");
        builder.Property(e => e.RecipientId).HasColumnName("recipient_id");
        builder.Property(e => e.CreatedById).HasColumnName("created_by_id");
        builder.Property(e => e.IsRead).HasColumnName("is_read");
        builder.Property(e => e.Message).HasColumnName("message");
        builder.Property(e => e.Title)
            .HasMaxLength(255)
            .HasColumnName("title");
        builder.Property(e => e.Type)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasColumnName("type");

        builder.HasOne(d => d.CreatedBy).WithMany(p => p.NotificationCreations)
            .HasForeignKey(d => d.CreatedById)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Notification_CreatedBy");
        
        builder.HasOne(d => d.Recipient).WithMany(p => p.NotificationRecipients)
            .HasForeignKey(d => d.RecipientId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Notification_Recipient");
    }
}