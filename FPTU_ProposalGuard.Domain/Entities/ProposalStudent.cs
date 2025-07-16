using System.Text.Json.Serialization;

namespace FPTU_ProposalGuard.Domain.Entities;

public class ProposalStudent
{
    public int ProposalStudentId { get; set; }
    public int ProjectProposalId { get; set; }

    public string? StudentCode { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? RoleInGroup { get; set; }
    [JsonIgnore]
    public ProjectProposal ProjectProposal { get; set; } = null!;
}
