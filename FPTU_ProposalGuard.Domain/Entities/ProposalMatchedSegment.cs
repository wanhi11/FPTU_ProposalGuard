namespace FPTU_ProposalGuard.Domain.Entities;

public class ProposalMatchedSegment
{
    public int SegmentId { get; set; }
    public int SimilarityId { get; set; }
    public string Context { get; set; } = null!;
    public string MatchContext { get; set; } = null!;
    public double MatchPercentage { get; set; }
    public ProposalSimilarity ProposalSimilarity { get; set; } = null!;
}