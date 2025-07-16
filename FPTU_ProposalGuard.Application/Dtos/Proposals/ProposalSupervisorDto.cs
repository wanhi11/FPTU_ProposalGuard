using System.Text.Json.Serialization;

namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class ProposalSupervisorDto
{
    public int ProposalSupervisorId { get; set; }
    public int ProjectProposalId { get; set; }

    public string? SupervisorNo { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? TitlePrefix { get; set; }

    [JsonIgnore]
    public ProjectProposalDto ProjectProposal { get; set; } = null!;
}

public class ExtractedProposalSupervisorDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}