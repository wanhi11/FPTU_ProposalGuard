

using System.Text.Json.Serialization;

namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class ProposalSimilarityDto
{
    
    public int SimilarityId { get; set; }

    public int HistoryId { get; set; }

    public int ExistingProposalId { get; set; }
    public decimal  MatchRatio{ get; set; }
    public int MatchCount { get; set; }
    public int LongestSequence { get; set; }
    public decimal OverallScore { get; set; }

    [JsonIgnore]
    public ProposalHistoryDto ProposalHistory { get; set; } = null!;

    [JsonIgnore]
    public ProjectProposalDto ExistingProposal { get; set; } = null!;
    
    public ICollection<ProposalMatchedSegmentDto> MatchedSegments { get; set; } = new List<ProposalMatchedSegmentDto>();
}