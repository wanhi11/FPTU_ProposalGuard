using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Application.Utils;
using FPTU_ProposalGuard.Domain.Common.Enums;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class ProposalHistoryService(
    ISystemMessageService msgService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,IGenericRepository<ProposalHistory, int> history)
    : GenericService<ProposalHistory, ProposalHistoryDto, int>(msgService, unitOfWork, mapper, logger),
        IProposalHistoryService<ProposalHistoryDto>
{
    private readonly IGenericRepository<ProposalHistory, int> _history = history;
    public async Task<IServiceResult> GetById(int id)
    {
        try
        {
            var baseSpec = new BaseSpecification<ProposalHistory>(x => x.HistoryId == id);
            // Include related entities
            baseSpec.ApplyInclude(q => q.Include(h => h.ProjectProposal)
                .ThenInclude(pp => pp.Submitter));

            var entity = await _unitOfWork.Repository<ProposalHistory, int>().GetWithSpecAsync(baseSpec);
            if (entity == null)
            {
                return new ServiceResult(ResultCodeConst.SYS_Warning0004,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004));
            }

            return new ServiceResult(ResultCodeConst.SYS_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
                _mapper.Map<ProposalHistoryDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when process get data");
        }
    }

    public async Task CreateWithoutSaveAsync(ProposalHistoryDto dto)
    {
        await _unitOfWork.Repository<ProposalHistory, int>()
            .AddAsync(_mapper.Map<ProposalHistory>(dto));
    }

    public override async Task<IServiceResult> UpdateAsync(int id, ProposalHistoryDto dto)
    {
        try
        {
            // Retrieve notification by id
            var baseSpec= new BaseSpecification<ProposalHistory>(x => x.HistoryId == id);
            baseSpec.ApplyInclude(q => q.Include(h => h.ProjectProposal)
                .ThenInclude(pp => pp.Approver)!);
            var existingEntity = await _unitOfWork.Repository<ProposalHistory, int>().GetWithSpecAsync(baseSpec);
            if (existingEntity == null)
            {
                // Not found {0}
                var errMsg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0002);
                return new ServiceResult(ResultCodeConst.SYS_Warning0002,
                    StringUtils.Format(errMsg, "lịch sử để tiến hành sửa đổi"));
            }
            
            // change props

            existingEntity.Status = dto.Status;
            existingEntity.ProjectProposal.Status =  dto.Status switch
            {
                "Approved" => ProjectProposalStatus.Approved,
                "Rejected" => ProjectProposalStatus.Rejected,
                "Pending" => ProjectProposalStatus.Pending,
                _ => existingEntity.ProjectProposal.Status 
            };
            existingEntity.ProjectProposal.ApproverId = dto.ProjectProposal.ApproverId;
            
            // Process update
            await _history.UpdateAsync(existingEntity);
            
            // Check if has changed or not
            if (!_history.HasChanges(existingEntity))
            {
                // Mark as update success
                return new ServiceResult(ResultCodeConst.SYS_Success0003,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003));
            }
            
            // Save DB
            if (await _unitOfWork.SaveChangesWithTransactionAsync() > 0)
            {
                // Mark as update success
                return new ServiceResult(ResultCodeConst.SYS_Success0003,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003));
            }
            
            // Mark as failed to update
            return new ServiceResult(ResultCodeConst.SYS_Fail0003,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Fail0003));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when process update History");
        }
    }
}