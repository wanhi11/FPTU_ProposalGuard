using FPTU_ProposalGuard.Application.Services;

namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class HandledProposalDto
{
    public string ProposalName { get; set; }
    public List<FieldPoint> Details { get; set; }
}

public class FieldPoint
{
    public string FieldName { get; set; }
    public double SimilarityPoint { get; set; }
    public string SimilarParagraph { get; set; }
}

// Class to map after extracting files
public class ExtractedProposal
{
    public ProposalContent Json { get; set; }
    public string Text { get; set; }
}

public class ProposalContent
{
    public string? Name { get; set; }
    public string? Context { get; set; }
    public string? Solution { get; set; }
}
public class ProposalWithText
{
    public ProjectProposalDto Proposal { get; set; }
    public string Text { get; set; }
}
public class ProposalAnalysisResult
{
    public string Name { get; set; }
    public string Context { get; set; }
    public string Solution { get; set; }
    public string Text { get; set; }
    public List<MatchReport> MatchedTopics { get; set; }
}

public record TopicMatch(
    int TopicId,
    int ChunkId,
    string Text,
    List<double> VectorEmbedding,
    double Score,
    int? OriginChunkId);

public record MatchReport(
    int TopicId,
    int MatchCount,
    int LongestContiguous,
    double MatchRatio,
    double AvgSimilarity,
    List<TopicMatch> Matches);
