using FPTU_ProposalGuard.API.Extensions;
using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.API.Payloads.Requests;
using FPTU_ProposalGuard.API.Payloads.Requests.Users;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Domain.Common.Enums;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Specifications;
using FPTU_ProposalGuard.Domain.Specifications.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FPTU_ProposalGuard.API.Controllers;

public class UserController : ControllerBase
{
    private readonly IUserService<UserDto> _userSvc;
    private readonly AppSettings _appSettings;

    public UserController(
        IUserService<UserDto> userSvc,
        IOptionsMonitor<AppSettings> monitor)
    {
        _userSvc = userSvc;
        _appSettings = monitor.CurrentValue;
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpGet(APIRoute.User.GetAll, Name = nameof(GetAllUserAsync))]
    public async Task<IActionResult> GetAllUserAsync([FromQuery] UserSpecParams specParams)
    {
        return Ok(await _userSvc.GetAllWithSpecAsync(new UserSpecification(
            specParams: specParams,
            pageIndex: specParams.PageIndex ?? 1,
            pageSize: specParams.PageSize ?? _appSettings.PageSize)));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpGet(APIRoute.User.GetById, Name = nameof(GetUserByIdAsync))]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
    {
        return Ok(await _userSvc.GetByIdAsync(id));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpPost(APIRoute.User.Create)]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest req)
    {
        return Ok(await _userSvc.CreateAsync(req.ToUserDto()));
    }

    [Authorize(Roles = nameof(Role.Administration))]
    [HttpPut(APIRoute.User.Update)]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid userId, [FromBody] UpdateUserRequest req)
    {
        return Ok(await _userSvc.UpdateAsync(userId, req.ToUserDto()));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpDelete(APIRoute.User.SoftDelete, Name = nameof(SoftDeleteUserAsync))]
    public async Task<IActionResult> SoftDeleteUserAsync([FromRoute] Guid id)
    {
        return Ok(await _userSvc.SoftDeleteAsync(id));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpDelete(APIRoute.User.SoftDeleteRange, Name = nameof(SoftDeleteRangeUserAsync))]
    public async Task<IActionResult> SoftDeleteRangeUserAsync([FromBody] RangeRequest<Guid> req)
    {
        return Ok(await _userSvc.SoftDeleteRangeAsync(req.Ids));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpDelete(APIRoute.User.UndoDelete, Name = nameof(UndoDeleteUserAsync))]
    public async Task<IActionResult> UndoDeleteUserAsync([FromRoute] Guid id)
    {
        return Ok(await _userSvc.UndoDeleteAsync(id));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpDelete(APIRoute.User.UndoDeleteRange, Name = nameof(UndoDeleteRangeUserAsync))]
    public async Task<IActionResult> UndoDeleteRangeUserAsync([FromBody] RangeRequest<Guid> req)
    {
        return Ok(await _userSvc.UndoDeleteRangeAsync(req.Ids));
    }
        
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpDelete(APIRoute.User.HardDelete,Name = nameof(DeleteUserAsync))]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
    {
        return Ok(await _userSvc.DeleteAsync(id));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpDelete(APIRoute.User.HardDeleteRange, Name = nameof(DeleteRangeUserAsync))]
    public async Task<IActionResult> DeleteRangeUserAsync([FromBody] RangeRequest<Guid> req)
    {
        return Ok(await _userSvc.DeleteRangeAsync(req.Ids));
    }
}