using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.Domain.Entities;

public class Semester
{
    public int SemesterId { get; set; }

    public string SemesterCode { get; set; } = null!;

    public int Year { get; set; }

    public Term Term { get; set; }

    public ICollection<ProjectProposal> ProjectProposals { get; set; } = new List<ProjectProposal>();
}
