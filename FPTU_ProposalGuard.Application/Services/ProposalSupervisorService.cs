using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Application.Utils;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using MapsterMapper;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class ProposalSupervisorService(
    IGenericRepository<ProposalSupervisor,int> supervisorRepository,
    ISystemMessageService msgService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger) : GenericService<ProposalSupervisor, ProposalSupervisorDto, int>(msgService, unitOfWork, mapper, logger)
    , IProposalSupervisorService<ProposalSupervisorDto>
{
    private readonly IGenericRepository<ProposalSupervisor,int> _supervisorRepository = supervisorRepository;
    public async Task<IServiceResult> GetByEmailAsync(string email)
    {
        try
        {
            var supervisorSpec = new BaseSpecification<ProposalSupervisor>(
                x => x.Email == email);
            var entity = await _supervisorRepository.GetWithSpecAsync(supervisorSpec);

            if (entity == null)
            {
                var errorMSg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0002);
                return new ServiceResult(
                    resultCode: ResultCodeConst.SYS_Warning0002,
                    message: StringUtils.Format(errorMSg, "email"));
            }

            return new ServiceResult(ResultCodeConst.SYS_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
                _mapper.Map<ProposalSupervisorDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get proposal supervisor by email");
        }
    }

    public async Task ModifyManyAsync(Dictionary<string, List<ProposalSupervisorDto>> modifyTask, int proposalId)
    {
        try
        {
            if (modifyTask == null || !modifyTask.Any())
            {
                return;
            }

            foreach (var task in modifyTask)
            {
                var action = task.Key;
                var dtos = task.Value;

                if (action.ToLower() == "create")
                {
                    var entities = dtos.Select(dto => _mapper.Map<ProposalSupervisor>(dto)).ToList();
                    await _unitOfWork.Repository<ProposalSupervisor,int>().AddRangeAsync(entities);
                }
                else if (action == "update")
                {
                    foreach (var dto in dtos)
                    {
                        var supervisorSpec = new BaseSpecification<ProposalSupervisor>(
                            x => x.ProjectProposalId == proposalId
                            && x.Email == dto.Email);
                        var entity = await _unitOfWork.Repository<ProposalSupervisor,int>().GetWithSpecAsync(supervisorSpec);
                        entity.Phone = dto.Phone;
                        entity.FullName = dto.FullName;
                        entity.TitlePrefix = dto.TitlePrefix;
                        
                        if (entity != null)
                        {
                            _mapper.Map(dto, entity);
                            await _unitOfWork.Repository<ProposalSupervisor,int>().UpdateAsync(entity);
                        }
                    }
                }
                else if (action == "delete")
                {
                    var ids = dtos.Select(dto => dto.ProposalSupervisorId).ToArray();
                    await  _unitOfWork.Repository<ProposalSupervisor,int>().DeleteRangeAsync(ids);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress modify many proposal supervisors");
        }
    }
}