namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class ProposalMatchedSegmentDto
{
    public int SegmentId { get; set; }
    public int SimilarityId { get; set; }
    public string Context { get; set; } = null!;
    public string MatchContext { get; set; } = null!;
    public double MatchPercentage { get; set; }
    public ProposalSimilarityDto ProposalSimilarity { get; set; } = null!;
}