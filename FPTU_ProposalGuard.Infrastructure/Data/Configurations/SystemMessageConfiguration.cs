using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class SystemMessageConfiguration : IEntityTypeConfiguration<SystemMessage>
{
    public void Configure(EntityTypeBuilder<SystemMessage> builder)
    {
        builder.HasKey(e => e.MsgId).HasName("PK_SystemMessage_MsgId");

        builder.ToTable("System_Message");
        
        builder.Property(e => e.MsgId)
            .HasColumnType("nvarchar(50)")
            .HasColumnName("msg_id");
        builder.Property(e => e.MsgContent)
            .HasMaxLength(1500)
            .HasColumnName("msg_content");
        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime")
            .HasColumnName("created_at");
        builder.Property(e => e.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(255);
        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime")
            .HasColumnName("updated_at");
        builder.Property(e => e.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(255);
    }
}