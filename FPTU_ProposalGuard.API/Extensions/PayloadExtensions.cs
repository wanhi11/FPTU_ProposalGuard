using FPTU_ProposalGuard.API.Payloads.Requests;
using FPTU_ProposalGuard.Application.Dtos;

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
}