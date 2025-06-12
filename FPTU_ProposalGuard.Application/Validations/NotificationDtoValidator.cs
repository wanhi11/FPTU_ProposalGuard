using FluentValidation;
using FPTU_ProposalGuard.Application.Dtos;

namespace FPTU_ProposalGuard.Application.Validations;

public class NotificationDtoValidator : AbstractValidator<NotificationDto>
{
    public NotificationDtoValidator()
    {
        RuleFor(n => n.Title);
        
        // Add more field validations
    }
}