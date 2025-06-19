using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface IRefreshTokenService<TDto> : IGenericService<RefreshToken, TDto, int>
    where TDto : class
{
    Task<IServiceResult> GetByUserIdAsync(Guid userId);
    Task<IServiceResult> GetByEmailAsync(string email);
    Task<IServiceResult> GetByTokenIdAndRefreshTokenIdAsync(string tokenId, string refreshTokenId);
}