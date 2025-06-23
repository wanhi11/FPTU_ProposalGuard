using FPTU_ProposalGuard.API.Payloads.Requests;
using FPTU_ProposalGuard.API.Payloads.Requests.Authentications;
using FPTU_ProposalGuard.API.Payloads.Requests.Notifications;
using FPTU_ProposalGuard.API.Payloads.Requests.Users;
using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Dtos.Users;

namespace FPTU_ProposalGuard.API.Extensions;

// Summary:
//		This class provide extensions method mapping from request payload to specific 
//		application objects
public static class PayloadExtensions
{
    #region Notifications
    public static NotificationDto ToNotificationDto(this CreateNotificationRequest req)
        => new()
        {
            Title = req.Title,
            Message = req.Message,
            Type = req.NotificationType,
            IsPublic = req.IsPublic
        };
    
    public static NotificationDto ToNotificationDto(this UpdateNotificationRequest req)
        => new()
        {
            Title = req.Title,
            Message = req.Message,
            Type = req.NotificationType
        };
    #endregion
    
    #region Users
    public static UserDto ToUserDto(this CreateUserRequest req)
    {
        return new()
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Dob = req.Dob,
            Phone = req.Phone,
            Address = req.Address,
            Gender = req.Gender.ToString(),
            RoleId = req.RoleId
        };
    }
    
    public static UserDto ToUserDto(this UpdateUserRequest req)
    {
        return new()
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Dob = req.Dob,
            Phone = req.Phone,
            Address = req.Address,
            Gender = req.Gender.ToString(),
            Avatar = req.Avatar
        };
    }
    
    public static UserDto ToUserDto(this UpdateProfileRequest req, string email)
    {
        return new()
        {
            Email = email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            Dob = req.Dob,
            Phone = req.Phone,
            Address = req.Address,
            Gender = req.Gender.ToString(),
            Avatar = req.Avatar
        };
    }
    #endregion
}