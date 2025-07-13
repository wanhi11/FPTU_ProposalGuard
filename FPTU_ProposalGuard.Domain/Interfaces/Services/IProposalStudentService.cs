using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface IProposalStudentService<TDto> : IGenericService<ProposalStudent, TDto, int>
    where TDto : class
{
    Task<IServiceResult> CreateManyAsync(IEnumerable<TDto> dtos);
}