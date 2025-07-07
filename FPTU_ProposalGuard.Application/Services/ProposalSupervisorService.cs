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
}