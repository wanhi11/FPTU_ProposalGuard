using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Application.Exceptions;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using FPTU_ProposalGuard.Domain.Specifications.Params;
using Mapster;
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
    private readonly IGenericRepository<ProjectProposal, int> _projectProposalRepository =
        unitOfWork.Repository<ProjectProposal, int>();

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
                .Include(pp => pp.ProposalSupervisors!)
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

    public override async Task<IServiceResult> UpdateAsync(int id, ProjectProposalDto dto)
    {
        // Initiate service result
        var serviceResult = new ServiceResult();

        try
        {
            // Retrieve the entity
            var existingEntity = await _unitOfWork.Repository<ProjectProposal, int>().GetByIdAsync(id);
            if (existingEntity == null)
            {
                var errMsg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004);
                return new ServiceResult(ResultCodeConst.SYS_Warning0004, errMsg);
            }

            // Process add update entity
            // Map properties from dto to existingEntity
            var localConfig = new TypeAdapterConfig();

            localConfig.NewConfig<ProjectProposalDto, ProjectProposal>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.ProposalHistories)
                .Ignore(dest => dest.ProposalStudents)
                .Ignore(dest => dest.ProposalSupervisors);
            dto.Adapt(existingEntity, localConfig);

            // Check if there are any differences between the original and the updated entity
            if (!_unitOfWork.Repository<ProjectProposal, int>().HasChanges(existingEntity))
            {
                serviceResult.ResultCode = ResultCodeConst.SYS_Success0003;
                serviceResult.Message = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003);
                serviceResult.Data = true;
                return serviceResult;
            }

            // Progress update when all require passed
            await _unitOfWork.Repository<ProjectProposal, int>().UpdateAsync(existingEntity);

            // Save changes to DB
            var rowsAffected = await _unitOfWork.SaveChangesAsync();
            if (rowsAffected == 0)
            {
                serviceResult.ResultCode = ResultCodeConst.SYS_Fail0003;
                serviceResult.Message = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Fail0003);
                serviceResult.Data = false;
            }

            // Mark as update success
            serviceResult.ResultCode = ResultCodeConst.SYS_Success0003;
            serviceResult.Message = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003);
            serviceResult.Data = true;
        }
        catch (UnprocessableEntityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw;
        }

        return serviceResult;
    }
}