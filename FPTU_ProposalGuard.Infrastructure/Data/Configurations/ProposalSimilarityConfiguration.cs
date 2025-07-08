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

        builder.Property(e => e.HistoryId).HasColumnName("history_id");

        builder.Property(e => e.ExistingProposalId).HasColumnName("existing_proposal_id");

        builder.Property(e => e.MatchRatio)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("match_ratio");

        builder.Property(e => e.MatchCount).HasColumnName("match_count");

        builder.Property(e => e.LongestSequence).HasColumnName("longest_sequence");

        builder.Property(e => e.OverallScore)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("overall_score");

        builder.HasOne(e => e.ProposalHistory)
            .WithMany(h => h.SimilarProposals)
            .HasForeignKey(e => e.HistoryId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ProposalSimilarity_HistoryId");

        builder.HasOne(e => e.ExistingProposal)
            .WithMany(p => p.ProposalSimilarityExistingProposals)
            .HasForeignKey(e => e.ExistingProposalId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProposalSimilarity_ExistingProposalId");

        builder.HasMany(e => e.MatchedSegments)
            .WithOne(s => s.ProposalSimilarity)
            .HasForeignKey(s => s.SimilarityId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ProposalMatchedSegment_SimilarityId");
    }
}