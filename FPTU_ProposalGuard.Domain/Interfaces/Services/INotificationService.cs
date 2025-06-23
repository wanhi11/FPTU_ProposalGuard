using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface INotificationService<TDto> : IGenericService<Notification, TDto, int>
    where TDto : class
{
    Task<IServiceResult> GetByIdAsync(int id, string? email = null);
    Task<IServiceResult> CreateAsync(string createdByEmail, TDto dto, List<string>? recipients = null);
}