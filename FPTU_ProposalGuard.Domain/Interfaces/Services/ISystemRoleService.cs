using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface ISystemRoleService<TDto> : IGenericService<SystemRole, TDto, int>
    where TDto : class
{
}