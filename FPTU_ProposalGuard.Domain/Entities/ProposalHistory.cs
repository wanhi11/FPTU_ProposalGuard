namespace FPTU_ProposalGuard.Domain.Entities;

public class ProposalHistory
{
    public int HistoryId { get; set; }

    public int ProjectProposalId { get; set; }

    public string OldStatus { get; set; } = null!;

    public string NewStatus { get; set; } = null!;

    public Guid ProcessById { get; set; }

    public DateTime ProcessDate { get; set; }

    public string? Comment { get; set; }

    public User ProcessBy { get; set; } = null!;

    public ProjectProposal ProjectProposal { get; set; } = null!;
}
