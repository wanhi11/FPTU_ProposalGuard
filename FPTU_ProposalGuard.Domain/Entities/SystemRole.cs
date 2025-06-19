namespace FPTU_ProposalGuard.Domain.Entities;

public class SystemRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;
    public string Description { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
