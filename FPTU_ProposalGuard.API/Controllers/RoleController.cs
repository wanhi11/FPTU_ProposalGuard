using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.Application.Dtos.SystemRoles;
using FPTU_ProposalGuard.Domain.Common.Enums;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTU_ProposalGuard.API.Controllers;

public class RoleController : ControllerBase
{
    private readonly ISystemRoleService<SystemRoleDto> _roleSvc;

    public RoleController(
        ISystemRoleService<SystemRoleDto> roleSvc)
    {
        _roleSvc = roleSvc;
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpGet(APIRoute.Role.GetAllRole, Name = nameof(GetAllRoleAsync))]
    public async Task<IActionResult> GetAllRoleAsync()
    {
        return Ok(await _roleSvc.GetAllAsync());
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpGet(APIRoute.Role.GetById, Name = nameof(GetRoleByIdAsync))]
    public async Task<IActionResult> GetRoleByIdAsync([FromRoute] int id)
    {
        return Ok(await _roleSvc.GetByIdAsync(id));
    }

    [Authorize(Roles = nameof(Role.Administration))]
    [HttpPatch(APIRoute.Role.UpdateUserRole, Name = nameof(UpdateUserRoleAsync))]
    public async Task<IActionResult> UpdateUserRoleAsync([FromRoute] Guid userId, [FromQuery] int roleId)
    {
        return Ok(await _roleSvc.UpdateUserRoleAsync(userId: userId, roleId: roleId));
    }
    
    [Authorize(Roles = nameof(Role.Administration))]
    [HttpDelete(APIRoute.Role.DeleteRole, Name = nameof(DeleteRoleAsync))]
    public async Task<IActionResult> DeleteRoleAsync([FromRoute] int id)
    {
        return Ok(await _roleSvc.DeleteAsync(id));
    }
}