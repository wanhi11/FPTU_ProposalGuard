namespace FPTU_ProposalGuard.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string RefreshTokenId { get; set; } = null!;

    public string TokenId { get; set; } = null!;

    public int RefreshCount { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
