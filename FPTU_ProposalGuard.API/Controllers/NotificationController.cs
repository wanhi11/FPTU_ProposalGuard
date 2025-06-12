using FPTU_ProposalGuard.API.Extensions;
using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.API.Payloads.Requests;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Specifications;
using FPTU_ProposalGuard.Domain.Specifications.Params;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FPTU_ProposalGuard.API.Controllers;

public class NotificationController : ControllerBase
{
    private readonly AppSettings _appSettings;
    private readonly INotificationService<NotificationDto> _nSvc;

    public NotificationController(
        INotificationService<NotificationDto> nSvc,
        IOptionsMonitor<AppSettings> monitor)
    {
        _nSvc = nSvc;
        _appSettings = monitor.CurrentValue;
    }

    [HttpGet(APIRoute.Notification.GetAll)]
    public async Task<IActionResult> GetAllNotificationAsync([FromQuery] NotificationSpecParams specParams)
    {
        return Ok(await _nSvc.GetAllWithSpecAsync(new NotificationSpecification(
                specParams: specParams,
                pageIndex: specParams.PageIndex ?? 1,
                pageSize: _appSettings.PageSize)));
    }

    [HttpPost(APIRoute.Notification.Create)]
    public async Task<IActionResult> CreateNotificationAsync([FromBody] CreateNotificationRequest req)
    {
        return Ok(await _nSvc.CreateAsync(req.ToNotificationDto()));
    }
}