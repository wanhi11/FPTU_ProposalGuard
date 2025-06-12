using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class ProposalStudentConfiguration : IEntityTypeConfiguration<ProposalStudent>
{
    public void Configure(EntityTypeBuilder<ProposalStudent> builder)
    {
        builder.HasKey(e => e.ProjectProposalId).HasName("PK_ProposalStudent_ProposalId");

        builder.ToTable("Proposal_Student");

        builder.Property(e => e.ProjectProposalId)
            .ValueGeneratedNever()
            .HasColumnName("project_proposal_id");
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .HasColumnName("email");
        builder.Property(e => e.FullName)
            .HasMaxLength(255)
            .HasColumnName("full_name");
        builder.Property(e => e.Phone)
            .HasMaxLength(50)
            .HasColumnName("phone");
        builder.Property(e => e.RoleInGroup)
            .HasMaxLength(50)
            .HasColumnName("role_in_group");
        builder.Property(e => e.StudentCode)
            .HasMaxLength(50)
            .HasColumnName("student_code");

        builder.HasOne(d => d.ProjectProposal).WithOne(p => p.ProposalStudent)
            .HasForeignKey<ProposalStudent>(d => d.ProjectProposalId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProposalStudent_ProposalId");
    }
}