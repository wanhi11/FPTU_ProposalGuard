using System.Text.Json.Serialization;

namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class ProposalStudentDto
{
    public int ProjectProposalId { get; set; }

    public string? StudentCode { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? RoleInGroup { get; set; }

    [JsonIgnore]
    public ProjectProposalDto ProjectProposal { get; set; } = null!;
}

public class ExtractedProposalStudentDto
{
    public string? FullName { get; set; }
    public string? StudentCode { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}