namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class ProposalAnalysisResult
{
    public string FileName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Context { get; set; } = null!;
    public string Solution { get; set; } = null!;
    public string Text { get; set; } = null!;
    public List<MatchReport> MatchedProposals { get; set; } = [];
}

public record ProposalMatch(
    int ProposalId,
    string Name,
    int ChunkId,
    string Text,
    double Score,
    double Similarity,
    int? OriginChunkId,
    string? UploadedChunkText
);

public record MatchReport(
    int ProposalId,
    string Name,
    int MatchCount,
    int LongestContiguous,
    double MatchRatio,
    double AvgSimilarity,
    List<ProposalMatch> Matches);