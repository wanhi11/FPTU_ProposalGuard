using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Exceptions;
using FPTU_ProposalGuard.Application.Extensions;
using FPTU_ProposalGuard.Application.Hubs;
using FPTU_ProposalGuard.Application.Utils;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using FPTU_ProposalGuard.Domain.Specifications.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class NotificationService : GenericService<Notification, NotificationDto, int>,
    INotificationService<NotificationDto>
{
    private readonly IGenericRepository<User, Guid> _userRepo;
    private readonly IGenericRepository<Notification, int> _notificationRepo;

    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        ISystemMessageService msgService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger) : base(msgService, unitOfWork, mapper, logger)
    {
        _userRepo = unitOfWork.Repository<User, Guid>();
        _notificationRepo = unitOfWork.Repository<Notification, int>();

        _hubContext = hubContext;
    }

    public override async Task<IServiceResult> GetAllWithSpecAsync(ISpecification<Notification> specification,
        bool tracked = true)
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

            if (nSpec.IsCallFromManagement)
            {
                // Apply include
                nSpec.ApplyInclude(q => q
                    .Include(n => n.NotificationRecipients)
                    .ThenInclude(nr => nr.Recipient)
                    .Include(n => n.CreatedBy)
                );
            }
            else
            {
                // Apply include
                nSpec.ApplyInclude(q => q
                    .Include(n => n.NotificationRecipients)
                    .ThenInclude(nr => nr.Recipient)
                    .Include(n => n.CreatedBy)
                );
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
            var entities = (await _unitOfWork.Repository<Notification, int>()
                .GetAllWithSpecAsync(nSpec, tracked)).ToList();
            if (entities.Any())
            {
                // Check whether is not call from management -> exclude all recipients are not belong to user
                if (!nSpec.IsCallFromManagement && !string.IsNullOrEmpty(nSpec.Email))
                {
                    var email = nSpec.Email;
                    foreach (var notification in entities)
                    {
                        if (notification.NotificationRecipients.Any())
                        {
                            notification.NotificationRecipients = notification.NotificationRecipients
                                .Where(r => Equals(r.Recipient.Email, email))
                                .Select(n => new NotificationRecipient()
                                {
                                    NotificationRecipientId = n.NotificationRecipientId,
                                    NotificationId = n.NotificationId,
                                    RecipientId = n.RecipientId,
                                    IsRead = n.IsRead
                                })
                                .ToList();
                        }
                    }
                }
                
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

    public async Task<IServiceResult> CreateAsync(
        string createdByEmail,
        NotificationDto dto,
        List<string>? recipients = null)
    {
        try
        {
            // Check created by existence
            var userSpec = new BaseSpecification<User>(u => Equals(u.Email, createdByEmail));
            var createdBy = await _userRepo.GetWithSpecAsync(userSpec);
            // Not found any match
            if (createdBy == null) throw new ForbiddenException("Not allow to access"); // Forbid to access

            // Validate inputs using the generic validator
            var validationResult = await ValidatorExtensions.ValidateAsync(dto);
            // Check for valid validations
            if (validationResult != null && !validationResult.IsValid)
            {
                // Convert ValidationResult to ValidationProblemsDetails.Errors
                var errors = validationResult.ToProblemDetails().Errors;
                throw new UnprocessableEntityException("Invalid Validations", errors);
            }

            // Current local datetime
            var currentLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                // Vietnam timezone
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            // Add create date
            dto.CreateDate = currentLocalDateTime;
            // Add created by
            dto.CreatedById = createdBy.UserId;

            // Add notification recipients (if any)
            if (recipients != null && recipients.Any(eml => !Equals(eml, createdBy.Email)) && !dto.IsPublic)
            {
                foreach (var email in recipients)
                {
                    // Try to retrieve user by email
                    var recipient = await _userRepo.GetWithSpecAsync(
                        new BaseSpecification<User>(u => Equals(email, u.Email)));
                    if (recipient == null) // Not found user match
                    {
                        // Msg: Not found user's email {0} to process send privacy notification
                        var msg = await _msgService.GetMessageAsync(ResultCodeConst.Notification_Warning0003);
                        return new ServiceResult(ResultCodeConst.Notification_Warning0003,
                            StringUtils.Format(msg, email));
                    }

                    // Add notification recipient
                    dto.NotificationRecipients.Add(new()
                    {
                        RecipientId = recipient.UserId,
                        IsRead = false
                    });
                }
            }

            // Map to entity
            var notificationEntity = _mapper.Map<Notification>(dto);
            // Add new entity
            await _notificationRepo.AddAsync(notificationEntity);
            // Save DB
            var isSaved = await _unitOfWork.SaveChangesAsync() > 0;
            if (isSaved)
            {
                await SendHubNotificationAsync(notificationDto: dto, recipients: recipients, isPublic: dto.IsPublic);

                // Response success
                return new ServiceResult(ResultCodeConst.Notification_Success0001,
                    await _msgService.GetMessageAsync(ResultCodeConst.Notification_Success0001));
            }

            // Msg: Failed to send notification
            return new ServiceResult(ResultCodeConst.Notification_Fail0001,
                await _msgService.GetMessageAsync(ResultCodeConst.Notification_Fail0001), false);
        }
        catch (ForbiddenException)
        {
            throw;
        }
        catch (UnprocessableEntityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
            throw new Exception("Error invokes when process create notification with collections of recipients");
        }
    }

    public override async Task<IServiceResult> UpdateAsync(int id, NotificationDto dto)
    {
        try
        {
            // Retrieve notification by id
            var existingEntity = await _unitOfWork.Repository<Notification, int>().GetByIdAsync(id);
            if (existingEntity == null)
            {
                // Not found {0}
                var errMsg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0002);
                return new ServiceResult(ResultCodeConst.SYS_Warning0002,
                    StringUtils.Format(errMsg, "thông báo để tiến hành sửa đổi"));
            }
            
            // Change props
            existingEntity.Title = dto.Title;
            existingEntity.Message = dto.Message;
            existingEntity.Type = dto.Type;
            
            // Process update
            await _notificationRepo.UpdateAsync(existingEntity);
            
            // Check if has changed or not
            if (!_notificationRepo.HasChanges(existingEntity))
            {
                // Mark as update success
                return new ServiceResult(ResultCodeConst.SYS_Success0003,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003));
            }
            
            // Save DB
            if (await _unitOfWork.SaveChangesAsync() > 0)
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
            throw new Exception("Error invoke when process update notification");
        }
    }

    public async Task<IServiceResult> GetByIdAsync(int id, string? email = null)
    {
        try
        {
            // Build spec
            var baseSpec = new BaseSpecification<Notification>(q => 
                q.NotificationId == id &&
                (
                    string.IsNullOrEmpty(email) || 
                    q.IsPublic || 
                    q.NotificationRecipients.Any(n => n.Recipient.Email == email) 
                ));

            if (string.IsNullOrEmpty(email)) // Call from management
            {
                // Apply include
                baseSpec.ApplyInclude(q => q
                    .Include(n => n.NotificationRecipients)
                    .ThenInclude(nr => nr.Recipient)
                    .Include(n => n.CreatedBy)
                );
            }
            else
            {
                // Apply include
                baseSpec.ApplyInclude(q => q
                    .Include(n => n.CreatedBy)
                );
            }
            
            // Retrieve notification with spec
            var existingEntity = await _unitOfWork.Repository<Notification, int>().GetWithSpecAsync(baseSpec);
            if (existingEntity == null)
            {
                // Data not found or empty
                return new ServiceResult(ResultCodeConst.SYS_Warning0004,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004));
            }

            return new ServiceResult(ResultCodeConst.SYS_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002), _mapper.Map<NotificationDto>(existingEntity));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when get notification by id");
        }
    }
    
    private async Task SendHubNotificationAsync(
        NotificationDto notificationDto,
        List<string>? recipients,
        bool isPublic)
    {
        // Build spec
        var userSpec = new BaseSpecification<User>();
        // Apply include
        userSpec.ApplyInclude(q => q.Include(u => u.Role));
        // Retrieve all users with spec
        var users = (await _userRepo.GetAllWithSpecAndSelectorAsync(
            specification: userSpec,
            selector: u => new User()
            {
                UserId = u.UserId,
                Email = u.Email
            })).ToList();
        // Exist at least one user
        if (users.Count > 0)
        {
            // Convert to list
            var userEmails = users.Select(u => u.Email).ToList();

            if (isPublic)
            {
                // Iterate each email to process send notification
                var tasks = userEmails
                    .Select(email => _hubContext.Clients.User(email).SendAsync("ReceiveNotification", new
                    {
                        NotificationId = notificationDto.NotificationId,
                        Title = notificationDto.Title,
                        Message = notificationDto.Message,
                        IsPublic = true,
                        Timestamp = notificationDto.CreateDate,
                        NotificationType = notificationDto.Type
                    }));

                await Task.WhenAll(tasks);
            }
            else if (recipients != null && recipients.Any())
            {
                var tasks = userEmails
                    .Where(email => recipients.Contains(email))
                    .Select(email => _hubContext.Clients.User(email).SendAsync("ReceiveNotification", new
                    {
                        NotificationId = notificationDto.NotificationId,
                        Title = notificationDto.Title,
                        Message = notificationDto.Message,
                        IsPublic = true,
                        Timestamp = notificationDto.CreateDate,
                        NotificationType = notificationDto.Type
                    }));

                await Task.WhenAll(tasks);
            }
        }
    }
}