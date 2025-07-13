namespace FPTU_ProposalGuard.API.Payloads.Requests.Proposals;

public class AddProposalsWithFilesRequest
{
    public List<CheckedFile> Files { get; set; } = new List<CheckedFile>();
    public int SemesterId { get; set; }
    
}

public class CheckedFile
{
    public IFormFile File { get; set; }
    public List<SimilarityDetail>? SimilarityDetails { get; set; }
}

public class ReUploadRequest
{
    public CheckedFile CheckedFileFile {get; set; } = null!;
    public int ProjectProposalId { get; set; }
    public int SemesterId { get; set; }
}

public class SimilarityDetail
{
    public int SimilarProposalId { get; set; }
    public int MatchCount { get; set; }
    public decimal MatchRatio { get; set; } 
    public int LongestContiguous { get; set; }
    public int OverallScore { get; set; }
    public List<Segment> Segments { get; set; } = new List<Segment>();
}
public class Segment
{
    public string Text { get; set; } = null!;
    public string UploadedChunkText { get; set; } = null!;
    public int Score { get; set; }
}
