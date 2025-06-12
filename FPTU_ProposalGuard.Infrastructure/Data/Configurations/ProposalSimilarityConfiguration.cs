using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class ProposalSimilarityConfiguration : IEntityTypeConfiguration<ProposalSimilarity>
{
    public void Configure(EntityTypeBuilder<ProposalSimilarity> builder)
    {
        builder.HasKey(e => e.SimilarityId).HasName("PK_ProposalSimilarity_SimilarityId");

        builder.ToTable("Proposal_Similarity");

        builder.Property(e => e.SimilarityId).HasColumnName("similarity_id");
        builder.Property(e => e.CheckedOn)
            .HasColumnType("datetime")
            .HasColumnName("checked_on");
        builder.Property(e => e.CheckedProposalId).HasColumnName("checked_proposal_id");
        builder.Property(e => e.ContextScore)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("context_score");
        builder.Property(e => e.ExistingProposalId).HasColumnName("existing_proposal_id");
        builder.Property(e => e.OverallScore)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("overall_score");
        builder.Property(e => e.SolutionScore)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("solution_score");
        builder.Property(e => e.TitleScore)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("title_score");

        builder.HasOne(d => d.CheckedProposal).WithMany(p => p.ProposalSimilarityCheckedProposals)
            .HasForeignKey(d => d.CheckedProposalId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProposalSimilaryty_CheckedProposalId");

        builder.HasOne(d => d.ExistingProposal).WithMany(p => p.ProposalSimilarityExistingProposals)
            .HasForeignKey(d => d.ExistingProposalId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProposalSimilaryty_ExistingProposalId");
    }
}