using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using MapsterMapper;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class ProjectProposalService(
    ISystemMessageService msgService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : GenericService<ProjectProposal, ProjectProposalDto, int>(msgService, unitOfWork, mapper, logger),
        IProjectProposalService<ProjectProposalDto>
{
    public async Task<IServiceResult> CreateManyAsync(List<ProjectProposalDto> dtos)
    {
        if (dtos.Count == 0)
        {
            return new ServiceResult(ResultCodeConst.SYS_Fail0001,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Fail0001));
        }

        var entities = _mapper.Map<List<ProjectProposal>>(dtos);
        await _unitOfWork.Repository<ProjectProposal, int>().AddRangeAsync(entities);
        await _unitOfWork.SaveChangesAsync();

        var returnDtos = _mapper.Map<List<ProjectProposalDto>>(entities);

        return new ServiceResult(ResultCodeConst.SYS_Success0001,
            await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0001), returnDtos);
    }
}