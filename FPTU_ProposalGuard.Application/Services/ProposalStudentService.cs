using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class ProposalStudentService(
    IGenericRepository<ProposalStudent,int> studentRepository,
    ISystemMessageService msgService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger) : GenericService<ProposalStudent, ProposalStudentDto, int>(msgService, unitOfWork, mapper, logger)
    , IProposalStudentService<ProposalStudentDto>
{
    public async Task<IServiceResult> CreateManyAsync(IEnumerable<ProposalStudentDto> dtos)
    {
        try
        {
            var proposalStudentDtos = dtos.ToList();
            if (!proposalStudentDtos.Any())
            {
                var errorMsg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004);
                return new ServiceResult(ResultCodeConst.SYS_Warning0004, errorMsg);
            }
        
            // remove duplicates and null or empty student codes
            var studentCodes = proposalStudentDtos.Select(dto => dto.StudentCode).Where(code => !string.IsNullOrEmpty(code)).Distinct().ToList();

            // check if there are any student codes to process
            var studentSpec = new BaseSpecification<ProposalStudent>(
                x => studentCodes.Contains(x.StudentCode));

            var existingStudents = (await _unitOfWork.Repository<ProposalStudent, int>()
                .GetAllWithSpecAsync(studentSpec)).ToList();

            // create a HashSet for fast lookup 
            var existingCodes = existingStudents.Select(s => s.StudentCode).ToHashSet();

            var newDtos = proposalStudentDtos
                .Where(dto => !existingCodes.Contains(dto.StudentCode))
                .ToList();

            var entities = newDtos.Select(dto => _mapper.Map<ProposalStudent>(dto)).ToList();
            await _unitOfWork.Repository<ProposalStudent, int>().AddRangeAsync(entities);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult(ResultCodeConst.SYS_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
                _mapper.Map<IEnumerable<ProposalStudentDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress create many proposal students");
        }
    }

    public async Task ModifyManyAsync(Dictionary<string, List<ProposalStudentDto>> modifyTask,int proposalId)
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
                    var entities = dtos.Select(dto => _mapper.Map<ProposalStudent>(dto)).ToList();
                    _unitOfWork.Repository<ProposalStudent, int>().AddRange(entities);
                }
                else if (action.ToLower() == "update")
                {
                    foreach (var dto in dtos)
                    {
                        var studentSpec = new BaseSpecification<ProposalStudent>(
                            x => x.StudentCode == dto.StudentCode && x.ProjectProposalId == proposalId);
                        var student = await _unitOfWork.Repository<ProposalStudent, int>()
                            .GetWithSpecAsync(studentSpec);
                        student.Email = dto.Email;
                        student.FullName = dto.FullName;
                        student.Phone = dto.Phone;
                        await _unitOfWork.Repository<ProposalStudent, int>().UpdateAsync(student);
                    }
                }
                else if (action.ToLower() == "delete")
                {
                    var studentCodes = dtos.Select(dto => dto.StudentCode).ToList();
                    var studentSpec = new BaseSpecification<ProposalStudent>(
                        x => studentCodes.Contains(x.StudentCode));
                    var studentsToDelete = _unitOfWork.Repository<ProposalStudent, int>()
                        .GetAllWithSpecAsync(studentSpec).Result.ToList();

                    await _unitOfWork.Repository<ProposalStudent, int>().DeleteRangeAsync(studentsToDelete.Select(s => s.ProposalStudentId)
                        .ToArray());
                }
            }

            await _unitOfWork.SaveChangesAsync() ;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress modify many proposal students");
        }
    }
}