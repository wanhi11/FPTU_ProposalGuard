using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class ProposalHistoryConfiguration : IEntityTypeConfiguration<ProposalHistory>
{
    public void Configure(EntityTypeBuilder<ProposalHistory> builder)
    {
        builder.HasKey(e => e.HistoryId).HasName("PK_ProposalHistory_HistoryId");

        builder.ToTable("Proposal_History");

        builder.Property(e => e.HistoryId).HasColumnName("history_id");
        builder.Property(e => e.Comment).HasColumnName("comment");
        builder.Property(e => e.NewStatus)
            .HasMaxLength(50)
            .HasColumnName("new_status");
        builder.Property(e => e.OldStatus)
            .HasMaxLength(50)
            .HasColumnName("old_status");
        builder.Property(e => e.ProcessById).HasColumnName("process_by_id");
        builder.Property(e => e.ProcessDate)
            .HasColumnType("datetime")
            .HasColumnName("process_date");
        builder.Property(e => e.ProjectProposalId).HasColumnName("project_proposal_id");

        builder.HasOne(d => d.ProcessBy).WithMany(p => p.ProposalHistories)
            .HasForeignKey(d => d.ProcessById)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("PK_ProposalHistory_ProcessById");

        builder.HasOne(d => d.ProjectProposal).WithMany(p => p.ProposalHistories)
            .HasForeignKey(d => d.ProjectProposalId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProposalHistory_ProjectProposalId");
    }
}