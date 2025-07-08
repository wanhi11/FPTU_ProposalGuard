namespace FPTU_ProposalGuard.Domain.Entities;

public class ProposalSimilarity
{
    public int SimilarityId { get; set; }

    public int HistoryId { get; set; }

    public int ExistingProposalId { get; set; }
    public decimal  MatchRatio{ get; set; }
    public int MatchCount { get; set; }
    public int LongestSequence { get; set; }
    public decimal OverallScore { get; set; }

    public ProposalHistory ProposalHistory { get; set; } = null!;

    public ProjectProposal ExistingProposal { get; set; } = null!;
    
    public ICollection<ProposalMatchedSegment> MatchedSegments { get; set; } = new List<ProposalMatchedSegment>();
}
