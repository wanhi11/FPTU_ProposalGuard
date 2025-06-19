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
            RecipientId = req.RecipientId,
            Title = req.Title,
            Message = req.Message,
            Type = req.Type
        };
    #endregion
    
    #region Users

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