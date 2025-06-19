using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Authentications;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using MapsterMapper;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class RefreshTokenService : GenericService<RefreshToken, RefreshTokenDto, int>,
    IRefreshTokenService<RefreshTokenDto>
{
    public RefreshTokenService(
        ISystemMessageService msgService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger)
        : base(msgService, unitOfWork, mapper, logger)
    {
    }

    public async Task<IServiceResult> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var refreshToken = await _unitOfWork.Repository<RefreshToken, int>().GetWithSpecAsync(
                new BaseSpecification<RefreshToken>(r => r.UserId.ToString().Equals(userId.ToString())));

            if (refreshToken is null)
            {
                return new ServiceResult(ResultCodeConst.SYS_Warning0004, "Data not found or empty");
            }

            return new ServiceResult(ResultCodeConst.SYS_Success0002, "Get data successfully",
                _mapper.Map<RefreshTokenDto>(refreshToken));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get user by id");
        }
    }

    public async Task<IServiceResult> GetByEmailAsync(string email)
    {
        try
        {
            // Check whether email belongs to user 
            var user = await _unitOfWork.Repository<User, Guid>()
                .GetWithSpecAsync(new BaseSpecification<User>(u => u.Email.Equals(email)));
            if (user != null)
            {
                // Retrieve refresh token
                var refreshToken = await _unitOfWork.Repository<RefreshToken, int>().GetWithSpecAsync(
                    new BaseSpecification<RefreshToken>(rft => rft.UserId == user.UserId));
                // Not exist refresh token
                if (refreshToken == null) return new ServiceResult(ResultCodeConst.SYS_Fail0002, "Fail to get data");

                // Response success
                return new ServiceResult(ResultCodeConst.SYS_Success0002, "Get data successfully",
                    _mapper.Map<RefreshTokenDto>(refreshToken));
            }

            return new ServiceResult(ResultCodeConst.SYS_Warning0004, "Data not found or empty");
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get user by email");
        }
    }

    public async Task<IServiceResult> GetByTokenIdAndRefreshTokenIdAsync(string tokenId, string refreshTokenId)
    {
        try
        {
            var refreshToken = await _unitOfWork.Repository<RefreshToken, int>().GetWithSpecAsync(
                new BaseSpecification<RefreshToken>(r => r.TokenId == tokenId
                                                         && r.RefreshTokenId == refreshTokenId));

            if (refreshToken is null)
            {
                return new ServiceResult(ResultCodeConst.SYS_Warning0004, "Data not found or empty or empty");
            }

            return new ServiceResult(ResultCodeConst.SYS_Success0002, "Get data successfully",
                _mapper.Map<RefreshTokenDto>(refreshToken));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get token and refresh token");
        }
    }
}