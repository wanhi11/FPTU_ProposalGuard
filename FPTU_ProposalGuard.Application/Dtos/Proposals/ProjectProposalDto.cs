using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.Application.Dtos.Proposals;

public class ProjectProposalDto
{
    public int ProjectProposalId { get; set; }

    public int SemesterId { get; set; }

    public string? VieTitle { get; set; } = null!;

    public string EngTitle { get; set; } = null!;

    public string? Abbreviation { get; set; } = null!;

    public string? SupervisorName { get; set; }

    public DateOnly DurationFrom { get; set; }

    public DateOnly DurationTo { get; set; }

    public string? Profession { get; set; }

    public string? SpecialtyCode { get; set; }

    public string? KindsOfPerson { get; set; }

    public string ContextText { get; set; } = null!;

    public string SolutionText { get; set; } = null!;

    public string? FunctionalRequirements { get; set; }

    public string? MainProposalContent { get; set; }

    public ProjectProposalStatus Status { get; set; }

    public Guid SubmitterId { get; set; }

    public Guid? ApproverId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public UserDto? Approver { get; set; }

    public UserDto Submitter { get; set; } = null!;
}