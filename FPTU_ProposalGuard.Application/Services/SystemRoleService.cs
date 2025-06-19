using FPTU_ProposalGuard.Application.Dtos.SystemRoles;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using MapsterMapper;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class SystemRoleService : GenericService<SystemRole, SystemRoleDto, int>, 
    ISystemRoleService<SystemRoleDto>
{
    public SystemRoleService(
        ISystemMessageService msgService,
        IUnitOfWork unitOfWork,
        IMapper mapper, ILogger logger)
        : base(msgService, unitOfWork, mapper, logger)
    {
    }
}