using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class SystemRoleConfiguration : IEntityTypeConfiguration<SystemRole>
{
    public void Configure(EntityTypeBuilder<SystemRole> builder)
    {
        builder.HasKey(e => e.RoleId).HasName("PK_SystemRole_RoleId");

        builder.ToTable("System_Role");

        builder.Property(e => e.RoleId).HasColumnName("role_id");
        builder.Property(e => e.NormalizedName)
            .HasMaxLength(155)
            .HasColumnName("normalized_name");
        builder.Property(e => e.Description)
            .HasMaxLength(155)
            .HasColumnName("description");
        builder.Property(e => e.RoleName)
            .HasMaxLength(155)
            .HasColumnName("role_name");
    }
}