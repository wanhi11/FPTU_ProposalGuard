using FPTU_ProposalGuard.Application.Dtos;
using MimeKit;

namespace FPTU_ProposalGuard.Application.Services.IExternalServices;

public interface IEmailService
{
    Task<MimeMessage> ConstructEmailMessageAsync(EmailMessageDto message, bool isBodyHtml);
    Task<bool> SendEmailAsync(EmailMessageDto message, bool isBodyHtml);
    Task<bool> SendAsync(MimeMessage mailMessage);
}