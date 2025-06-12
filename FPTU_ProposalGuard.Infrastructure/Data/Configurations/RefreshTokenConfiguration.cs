using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(e => e.Id).HasName("Pk_RefreshToken_Id");

        builder.ToTable("Refresh_Token");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateDate)
            .HasColumnType("datetime")
            .HasColumnName("create_date");
        builder.Property(e => e.ExpiryDate)
            .HasColumnType("datetime")
            .HasColumnName("expiry_date");
        builder.Property(e => e.RefreshCount).HasColumnName("refresh_count");
        builder.Property(e => e.RefreshTokenId)
            .HasMaxLength(100)
            .HasColumnName("refresh_token_id");
        builder.Property(e => e.TokenId)
            .HasMaxLength(36)
            .HasColumnName("token_id");
        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RefreshToken_UserId");
    }
}