using FPTU_ProposalGuard.Domain.Common.Enums;

namespace FPTU_ProposalGuard.API.Payloads.Requests.Users;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime? Dob { get; set; }
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public Gender Gender { get; set; }
    public string? Avatar { get; set; } 
}