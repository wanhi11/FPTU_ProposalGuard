using FluentValidation;
using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Application.Dtos.Notifications;

namespace FPTU_ProposalGuard.Application.Validations;

public class NotificationDtoValidator : AbstractValidator<NotificationDto>
{
    public NotificationDtoValidator()
    {
        RuleFor(n => n.Title);
        
        // Add more field validations
    }
}