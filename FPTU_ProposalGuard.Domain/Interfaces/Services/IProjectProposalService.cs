using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface IProjectProposalService <TDto> : IGenericService<ProjectProposal, TDto, int>
    where TDto : class
{
    Task<IServiceResult> CreateManyAsync(List<TDto> dtos);  
}