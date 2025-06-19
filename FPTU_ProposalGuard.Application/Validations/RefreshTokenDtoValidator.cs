using FluentValidation;
using FPTU_ProposalGuard.Application.Dtos.Authentications;

namespace FPTU_ProposalGuard.Application.Validations;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
    }
}