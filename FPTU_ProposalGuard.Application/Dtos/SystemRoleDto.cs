namespace FPTU_ProposalGuard.Application.Dtos;

public class SystemRoleDto
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;
}