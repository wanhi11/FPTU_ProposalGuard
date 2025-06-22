namespace FPTU_ProposalGuard.API.Payloads.Requests.Roles;

public class CreateRoleRequest
{
    public string RoleName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
}