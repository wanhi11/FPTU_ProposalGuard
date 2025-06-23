using FluentValidation;
using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Application.Dtos.Notifications;

namespace FPTU_ProposalGuard.Application.Validations;

public class NotificationDtoValidator : AbstractValidator<NotificationDto>
{
    public NotificationDtoValidator()
    {
        // Title must not be null or empty
        RuleFor(notification => notification.Title)
            .NotNull()
            .WithMessage("Yêu cầu nhập tiêu đề")
            .NotEmpty()
            .WithMessage("Tiêu đề không được phép rỗng")
            .MaximumLength(150)
            .WithMessage("Tiêu đề thông báo cho phép tối đa 150 ký tự");
            
        // Message must not be null or empty
        RuleFor(notification => notification.Message)
            .NotNull()
            .WithMessage("Yêu cầu nhập nội dung")
            .NotEmpty()
            .WithMessage("Nội dung không được phép rỗng")
            .MaximumLength(4000)
            .WithMessage("Nội dung thông báo cho phép tối đa 4000 ký tự");
    }
}