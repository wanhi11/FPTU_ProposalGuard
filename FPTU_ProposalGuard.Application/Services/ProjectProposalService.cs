using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using FPTU_ProposalGuard.Domain.Specifications.Params;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class ProjectProposalService(
    ISystemMessageService msgService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IGenericRepository<ProjectProposal, int> projectProposalRepository,
    ILogger logger)
    : GenericService<ProjectProposal, ProjectProposalDto, int>(msgService, unitOfWork, mapper, logger),
        IProjectProposalService<ProjectProposalDto>
{
    private readonly IGenericRepository<ProjectProposal, int> _projectProposalRepository= unitOfWork.Repository<ProjectProposal, int>();
    public async Task<IServiceResult> CreateManyAsync(List<ProjectProposalDto> dtos)
    {
        if (dtos.Count == 0)
        {
            return new ServiceResult(ResultCodeConst.SYS_Fail0001,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Fail0001));
        }
        try
        {
            var entities = _mapper.Map<List<ProjectProposal>>(dtos);
        
            await _unitOfWork.Repository<ProjectProposal, int>().AddRangeAsync(entities);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            var returnDtos = _mapper.Map<List<ProjectProposalDto>>(entities);

            return new ServiceResult(ResultCodeConst.SYS_Success0001,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0001), returnDtos);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create project proposals.");
            return new ServiceResult(ResultCodeConst.SYS_Fail0001,
                "Lỗi khi tạo Project Proposals: " + ex.Message);
        }
    }
    
    public override async Task<IServiceResult> GetByIdAsync(int id)
    {
        var baseSpec = new BaseSpecification<ProjectProposal>(pp => pp.ProjectProposalId == id);
        
        // Apply include
        baseSpec.ApplyInclude(q => 
            q.Include(pp => pp.ProposalHistories)
            .Include(pp=> pp.ProposalSupervisors!)
            .Include(pp => pp.ProposalStudents!));
        
        var entity = await _unitOfWork.Repository<ProjectProposal, int>().GetWithSpecAsync(baseSpec);
        if (entity == null)
        {
            return new ServiceResult(ResultCodeConst.SYS_Warning0004,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004));
        }
        return new ServiceResult(ResultCodeConst.SYS_Success0002,
            await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
            _mapper.Map<ProjectProposalDto>(entity));

    }
}