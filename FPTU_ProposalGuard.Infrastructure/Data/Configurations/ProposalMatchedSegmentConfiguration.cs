using FPTU_ProposalGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPTU_ProposalGuard.Infrastructure.Data.Configurations;

public class ProposalMatchedSegmentConfiguration:IEntityTypeConfiguration<ProposalMatchedSegment>
{
    public void Configure(EntityTypeBuilder<ProposalMatchedSegment> builder)
    {
        builder.HasKey(e => e.SegmentId).HasName("PK_ProposalMatchedSegment_SegmentId");

        builder.ToTable("Proposal_Matched_Segment");

        builder.Property(e => e.SegmentId).HasColumnName("segment_id");

        builder.Property(e => e.SimilarityId).HasColumnName("similarity_id");

        builder.Property(e => e.Context)
            .IsRequired()
            .HasColumnName("context");

        builder.Property(e => e.MatchContext)
            .IsRequired()
            .HasColumnName("match_context");

        builder.Property(e => e.MatchPercentage)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("match_percentage");

        builder.HasOne(s => s.ProposalSimilarity)
            .WithMany(sim => sim.MatchedSegments)
            .HasForeignKey(s => s.SimilarityId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ProposalMatchedSegment_SimilarityId");
    }
}