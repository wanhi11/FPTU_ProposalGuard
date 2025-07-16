using System.Text.Json.Serialization;

namespace FPTU_ProposalGuard.Domain.Entities;

public class ProposalSupervisor
{
    public int ProposalSupervisorId { get; set; }
    public int ProjectProposalId { get; set; }

    public string? SupervisorNo { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? TitlePrefix { get; set; }
    [JsonIgnore]
    public ProjectProposal ProjectProposal { get; set; } = null!;
}
