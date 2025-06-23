using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface INotificationRecipientService<TDto> : IGenericService<NotificationRecipient,TDto, int> 
    where TDto: class
{
    Task<IServiceResult> GetNumberOfUnreadNotificationsAsync(string email);
    Task<IServiceResult> UpdateRangeReadStatusAsync(string email, List<int> notificationIds);
    Task<IServiceResult> MarkAsReadAllAsync(string email);
}