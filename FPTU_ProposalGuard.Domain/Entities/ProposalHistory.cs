namespace FPTU_ProposalGuard.Domain.Entities;

public class ProposalHistory
{
    public int HistoryId { get; set; }

    public int ProjectProposalId { get; set; }

    public string Status { get; set; } = null!;

    public int Version { get; set; }
    public string Url { get; set; } = null!;
    public Guid ProcessById { get; set; }

    public DateTime ProcessDate { get; set; }

    public string? Comment { get; set; }

    public User ProcessBy { get; set; } = null!;

    public ProjectProposal ProjectProposal { get; set; } = null!;
    public ICollection<ProposalSimilarity> SimilarProposals { get; set; } = new List<ProposalSimilarity>();
}
