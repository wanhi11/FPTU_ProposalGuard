namespace FPTU_ProposalGuard.Domain.Entities;

public class ProposalSimilarity
{
    public int SimilarityId { get; set; }

    public int CheckedProposalId { get; set; }

    public int ExistingProposalId { get; set; }

    public decimal TitleScore { get; set; }

    public decimal ContextScore { get; set; }

    public decimal SolutionScore { get; set; }

    public decimal OverallScore { get; set; }

    public DateTime CheckedOn { get; set; }

    public ProjectProposal CheckedProposal { get; set; } = null!;

    public ProjectProposal ExistingProposal { get; set; } = null!;
}
