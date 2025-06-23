using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Application.Exceptions;
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

public class NotificationRecipientService : GenericService<NotificationRecipient, NotificationRecipientDto, int>,
    INotificationRecipientService<NotificationRecipientDto>
{
    private readonly IGenericRepository<User, Guid> _userRepo;
    private readonly IGenericRepository<NotificationRecipient, int> _notificationRecipientRepo;

    public NotificationRecipientService(ILogger logger,
        ISystemMessageService msgService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserService<UserDto> userService) : base(msgService, unitOfWork, mapper, logger)
    {
        _userRepo = _unitOfWork.Repository<User, Guid>();
        _notificationRecipientRepo = _unitOfWork.Repository<NotificationRecipient, int>();
    }

    public async Task<IServiceResult> GetNumberOfUnreadNotificationsAsync(string email)
    {
        try
        {
            var user = await _userRepo.GetWithSpecAsync(new BaseSpecification<User>(u =>
                Equals(u.Email, email)));
            if (user == null) throw new ForbiddenException("Not allow to access");

            // Build spec
            var baseSpec = new BaseSpecification<NotificationRecipient>(n =>
                n.RecipientId == user.UserId && n.IsRead == false);
            // Include notification role
            baseSpec.ApplyInclude(q =>
                q.Include(u => u.Notification));

            // Return with count number
            return new ServiceResult(ResultCodeConst.SYS_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
                await _notificationRecipientRepo.CountAsync(baseSpec));
        }
        catch (ForbiddenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when process get number of unread notifications");
        }
    }

    public async Task<IServiceResult> MarkAsReadAllAsync(string email)
    {
        try
        {
            // Build spec
            var baseSpec = new BaseSpecification<NotificationRecipient>(n => n.Recipient.Email == email);
            // Retrieve all with spec
            var entities = (await _notificationRecipientRepo.GetAllWithSpecAsync(baseSpec)).ToList();
            if (entities.Any())
            {
                // Iterate each notification recipient to update read status
                foreach (var noti in entities)
                {
                    // Change read status
                    noti.IsRead = true;
                    // Process update
                    await _notificationRecipientRepo.UpdateAsync(noti);
                }
            }

            // Process save DB
            await _unitOfWork.SaveChangesAsync();

            // Always mark as success
            return new ServiceResult(ResultCodeConst.SYS_Success0003,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when mark notification read all");
        }
    }

    public async Task<IServiceResult> UpdateRangeReadStatusAsync(string email, List<int> notificationIds)
    {
        try
        {
            // Build spec
            var baseSpec = new BaseSpecification<NotificationRecipient>(r =>
                notificationIds.Contains(r.NotificationId) && r.Recipient.Email == email);
            // Retrieve all with spec
            var entities = (await _notificationRecipientRepo
                .GetAllWithSpecAsync(baseSpec)).ToList();
            if (entities.Any())
            {
                // Iterate each notification recipient to update read status
                foreach (var noti in entities)
                {
                    // Change read status
                    noti.IsRead = true;
                    // Process update
                    await _notificationRecipientRepo.UpdateAsync(noti);
                }
            }

            // Process save DB
            await _unitOfWork.SaveChangesAsync();

            // Always mark as success
            return new ServiceResult(ResultCodeConst.SYS_Success0003,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when process update range read status");
        }
    }
}