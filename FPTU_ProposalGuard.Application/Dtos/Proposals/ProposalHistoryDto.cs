using FPTU_ProposalGuard.Application.Dtos.Users;

namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class ProposalHistoryDto
{
    public int HistoryId { get; set; }

    public int ProjectProposalId { get; set; }

    public string Status { get; set; } = null!;

    public int Version { get; set; }
    public string Url { get; set; } = null!;
    public Guid ProcessById { get; set; }

    public DateTime ProcessDate { get; set; }

    public string? Comment { get; set; }

    public UserDto ProcessBy { get; set; } = null!;

    public ProjectProposalDto ProjectProposal { get; set; } = null!;
    public ICollection<ProposalSimilarityDto> SimilarProposals { get; set; } = new List<ProposalSimilarityDto>();
}