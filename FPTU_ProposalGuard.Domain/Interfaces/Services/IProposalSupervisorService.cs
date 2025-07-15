using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface IProposalSupervisorService<TDto> : IGenericService<ProposalSupervisor, TDto, int>
    where TDto : class
{
    Task<IServiceResult> GetByEmailAsync(string email);
    Task ModifyManyAsync(Dictionary<string,List<TDto>> modifyTask,int proposalId);
}