using FPTU_ProposalGuard.Domain.Specifications.Params;

namespace FPTU_ProposalGuard.Domain.Specifications;

public class ProposalSpecParams : BaseSpecParams
{
    public string? Title { get; set; }
    public string? Abbreviation  { get; set; }
    public string? SemesterCode { get; set; }
    public DateOnly?[]? DurationFromRange { get; set; } 
    public DateOnly?[]? DurationToRange { get; set;}
    public DateTime?[]? CreateDateRange { get; set;}
    public string? Status { get; set; }
    public string? SupervisorName { get; set; }
    public string? SupervisorEmail { get; set; }
    public string? StudentName { get; set; }
    public string? StudentCode { get; set; }
    public string? StudentEmail { get; set; }

}