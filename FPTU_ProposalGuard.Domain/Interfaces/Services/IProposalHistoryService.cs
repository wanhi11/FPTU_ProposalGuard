using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface IProposalHistoryService<TDto> : IGenericService<ProposalHistory, TDto, int>
    where TDto : class
{
    Task<IServiceResult> GetById(int id);
}