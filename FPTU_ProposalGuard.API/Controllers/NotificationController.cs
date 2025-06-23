using System.Security.Claims;
using CloudinaryDotNet.Actions;
using FPTU_ProposalGuard.API.Extensions;
using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.API.Payloads.Requests;
using FPTU_ProposalGuard.API.Payloads.Requests.Notifications;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Specifications;
using FPTU_ProposalGuard.Domain.Specifications.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Role = FPTU_ProposalGuard.Domain.Common.Enums.Role;

namespace FPTU_ProposalGuard.API.Controllers;

public class NotificationController : ControllerBase
{
    private readonly AppSettings _appSettings;
    private readonly INotificationService<NotificationDto> _nSvc;
    private readonly INotificationRecipientService<NotificationRecipientDto> _nRecipientSvc;

    public NotificationController(
        INotificationService<NotificationDto> nSvc,
        INotificationRecipientService<NotificationRecipientDto> nRecipientSvc,
        IOptionsMonitor<AppSettings> monitor)
    {
        _nSvc = nSvc;
        _nRecipientSvc = nRecipientSvc;
        _appSettings = monitor.CurrentValue;
    }

    #region Management
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpGet(APIRoute.Notification.GetAll)]
    public async Task<IActionResult> GetAllNotificationAsync([FromQuery] NotificationSpecParams specParams)
    {
        return Ok(await _nSvc.GetAllWithSpecAsync(new NotificationSpecification(
            specParams: specParams, 
            pageIndex: specParams.PageIndex ?? 1,
            pageSize: specParams.PageSize ?? _appSettings.PageSize,
            isCallFromManagement: true)));
    }

    [Authorize(Roles = nameof(Role.Administration))]
    [HttpPost(APIRoute.Notification.Create, Name=nameof(CreateNotificationAsync))]
    public async Task<IActionResult> CreateNotificationAsync([FromBody] CreateNotificationRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _nSvc.CreateAsync(
            createdByEmail: email ?? string.Empty, 
            dto: req.ToNotificationDto(),
            recipients: req.ListRecipient));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpGet(APIRoute.Notification.GetById, Name = nameof(GetNotificationByIdAsync))]
    public async Task<IActionResult> GetNotificationByIdAsync([FromRoute] int id)
    {
        return Ok(await _nSvc.GetByIdAsync(id: id, email: null));
    }
    #endregion
    
    [Authorize] 
    [HttpGet(APIRoute.Notification.GetAllPrivacy, Name = nameof(GetAllPrivacyNotificationAsync))]
    public async Task<IActionResult> GetAllPrivacyNotificationAsync([FromQuery] NotificationSpecParams specParams)
    {
        // Assign email to spec params 
        specParams.Email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _nSvc.GetAllWithSpecAsync(new NotificationSpecification(
                specParams: specParams, 
                pageIndex: specParams.PageIndex ?? 1,
                pageSize: specParams.PageSize ?? _appSettings.PageSize,
                isCallFromManagement: false)));
    }
    
    [Authorize]
    [HttpGet(APIRoute.Notification.GetNumberOfUnreadNotifications, Name = nameof(GetNumberOfUnreadNotifications))]
    public async Task<IActionResult> GetNumberOfUnreadNotifications()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _nRecipientSvc.GetNumberOfUnreadNotificationsAsync(email ?? string.Empty));
    }

    [Authorize]
    [HttpPut(APIRoute.Notification.Update, Name = nameof(UpdateNotificationAsync))]
    public async Task<IActionResult> UpdateNotificationAsync([FromRoute] int id, [FromBody] UpdateNotificationRequest req)
    {
        return Ok(await _nSvc.UpdateAsync(id, req.ToNotificationDto()));
    }
    
    [Authorize]
    [HttpPut(APIRoute.Notification.UpdateReadStatus, Name = nameof(UpdateReadStatusAsync))]
    public async Task<IActionResult> UpdateReadStatusAsync([FromBody] UpdateRangeReadStatusRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _nRecipientSvc.UpdateRangeReadStatusAsync(
            email: email ?? string.Empty,
            notificationIds: req.NotificationIds));
    }

    [Authorize]
    [HttpPut(APIRoute.Notification.MarkAsReadAll, Name = nameof(MarkAsReadAllAsync))]
    public async Task<IActionResult> MarkAsReadAllAsync()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _nRecipientSvc.MarkAsReadAllAsync(email: email ?? string.Empty));
    }
    
    [Authorize]
    [HttpGet(APIRoute.Notification.GetPrivacyById, Name = nameof(GetPrivacyByIdAsync))]
    public async Task<IActionResult> GetPrivacyByIdAsync([FromRoute] int id)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _nSvc.GetByIdAsync(id: id, email: email ?? string.Empty));
    }
}