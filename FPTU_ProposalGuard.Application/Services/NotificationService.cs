using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using FPTU_ProposalGuard.Domain.Specifications.Interfaces;
using MapsterMapper;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class NotificationService : GenericService<Notification, NotificationDto, int>, 
    INotificationService<NotificationDto>
{
    public NotificationService(
        ISystemMessageService msgService, 
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ILogger logger) : base(msgService, unitOfWork, mapper, logger)
    {
    }

    public override async Task<IServiceResult> GetAllWithSpecAsync(ISpecification<Notification> specification, bool tracked = true)
    {
        try
        {
            // Try to parse specification
            var nSpec = specification as NotificationSpecification;
            // Check if specification is null
            if (nSpec == null)
            {
                return new ServiceResult(ResultCodeConst.SYS_Fail0002,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Fail0002));
            }
            
            // Count total authors
            var totalWithSpec = await _unitOfWork.Repository<Notification, int>().CountAsync(nSpec);
            // Count total page
            var totalPage = (int)Math.Ceiling((double)totalWithSpec / nSpec.PageSize);
			
            // Set pagination to specification after count total notifications
            if (nSpec.PageIndex > totalPage 
                || nSpec.PageIndex < 1) // Exceed total page or page index smaller than 1
            {
                nSpec.PageIndex = 1; // Set default to first page
            }
            
            // Apply pagination
            nSpec.ApplyPaging(
                skip: nSpec.PageSize * (nSpec.PageIndex - 1), 
                take: nSpec.PageSize);
			
            // Get all with spec
            var entities = await _unitOfWork.Repository<Notification, int>()
                .GetAllWithSpecAsync(nSpec, tracked);

            if (entities.Any())
            {
                // Convert to dto collection 
                var nDtos = _mapper.Map<IEnumerable<NotificationDto>>(entities);
                
                // Pagination result 
                var paginationResultDto = new PaginatedResultDto<NotificationDto>(nDtos,
                    nSpec.PageIndex, nSpec.PageSize, totalPage, totalWithSpec);
				
                // Response with pagination 
                return new ServiceResult(
                    resultCode: ResultCodeConst.SYS_Success0002, 
                    message: await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
                    data: paginationResultDto);
            }
            
            // Not found any data
            return new ServiceResult(
                resultCode: ResultCodeConst.SYS_Warning0004, 
                message: await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004),
                data: _mapper.Map<IEnumerable<NotificationDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get all notifications");
        }
    }
}