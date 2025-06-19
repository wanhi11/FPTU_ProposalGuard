using System.Security.Claims;
using FPTU_ProposalGuard.API.Extensions;
using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.API.Payloads.Requests.Authentications;
using FPTU_ProposalGuard.API.Payloads.Requests.Users;
using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FPTU_ProposalGuard.API.Controllers;

public class AuthenticationController : ControllerBase
{
    private readonly IUserService<UserDto> _userSvc;

    public AuthenticationController(IUserService<UserDto> userSvc)
    {
        _userSvc = userSvc;
    }

    [Authorize]
    [HttpGet(APIRoute.Authentication.CurrentUser, Name = nameof(GetCurrentUserAsync))]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        // Retrieve user email from token
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(await _userSvc.GetCurrentUserAsync(email ?? string.Empty));
    }
    
    [HttpPost(APIRoute.Authentication.SignIn, Name = nameof(SignInAsync))]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest req)
    {
        return Ok(await _userSvc.SignInAsync(req.Email));
    }
		
    [HttpPost(APIRoute.Authentication.SignInWithPassword, Name = nameof(SignInWithPasswordAsync))]
    public async Task<IActionResult> SignInWithPasswordAsync([FromBody] SignInWithPasswordRequest req)
    {
        return Ok(await _userSvc.SignInWithPasswordAsync(email: req.Email, pwd: req.Password));
    }
		
    [HttpPost(APIRoute.Authentication.SignInWithOtp, Name = nameof(SignInWithOtpAsync))]
    public async Task<IActionResult> SignInWithOtpAsync([FromBody] SignInWithOtpRequest req)
    {
        return Ok(await _userSvc.SignInWithOtpAsync(email: req.Email, otp: req.Otp));
    }
    
    [HttpPost(APIRoute.Authentication.SignInWithGoogle, Name = nameof(SignInWithGoogleAsync))]
    public async Task<IActionResult> SignInWithGoogleAsync([FromBody] GoogleAuthRequest req)
    {
        return Ok(await _userSvc.SignInWithGoogleAsync(req.Code));
    }
    
    [HttpPost(APIRoute.Authentication.ResendOtp, Name = nameof(ResendOtpForSignUpAsync))]
    public async Task<IActionResult> ResendOtpForSignUpAsync([FromBody] ResendOtpForSignUpRequest req)
    {
        return Ok(await _userSvc.ResendOtpAsync(req.Email));
    }
    
    [HttpGet(APIRoute.Authentication.ForgotPassword, Name = nameof(ForgotPasswordAsync))]
    public async Task<IActionResult> ForgotPasswordAsync([FromQuery] ForgotPasswordRequest req)
    {
        return Ok(await _userSvc.ForgotPasswordAsync(req.Email));
    }

    [HttpPatch(APIRoute.Authentication.ChangePassword, Name = nameof(ChangePasswordAsync))]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest req)
    {
        return Ok(await _userSvc.ChangePasswordAsync(req.Email, req.Password, req.Token));
    }
    
    [HttpPost(APIRoute.Authentication.ChangePasswordOtpVerification, Name = nameof(ChangePasswordOtpVerificationAsync))]
    public async Task<IActionResult> ChangePasswordOtpVerificationAsync([FromBody] OtpVerificationRequest req)
    {
        return Ok(await _userSvc.VerifyChangePasswordOtpAsync(req.Email, req.Otp));
    }

    [HttpPost(APIRoute.Authentication.RefreshToken, Name = nameof(RefreshTokenAsync))]
	public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest req)
	{
		// Generate new token using refresh token
		return Ok(await _userSvc.RefreshTokenAsync(req.AccessToken, req.RefreshToken));
	}

	[HttpPost(APIRoute.Authentication.EnableMfa, Name = nameof(EnableMfaAsync))]
	public async Task<IActionResult> EnableMfaAsync([FromBody] EnableMfaRequest req)
	{
		return Ok(await _userSvc.EnableMfaAsync(req.Email));
	}
	
	[HttpPost(APIRoute.Authentication.ValidateMfa, Name = nameof(ValidateMfaAsync))]
	public async Task<IActionResult> ValidateMfaAsync([FromBody] ValidateMfaRequest req)
	{
		return Ok(await _userSvc.ValidateMfaAsync(req.Email, req.Otp));
	}
	
	[HttpPost(APIRoute.Authentication.ValidateBackupCode, Name = nameof(ValidateMfaBackupCodeAsync))]
	public async Task<IActionResult> ValidateMfaBackupCodeAsync([FromBody] ValidateMfaBackupCodeRequest req)
	{
		return Ok(await _userSvc.ValidateMfaBackupCodeAsync(req.Email, req.BackupCode));
	}

	[Authorize]
	[HttpPost(APIRoute.Authentication.RegenerateBackupCode, Name = nameof(RegenerateBackupCodeAsync))]
	public async Task<IActionResult> RegenerateBackupCodeAsync()
	{
		// Retrieve user email from token
		var email = User.FindFirst(ClaimTypes.Email)?.Value;
		return Ok(await _userSvc.RegenerateMfaBackupCodeAsync(email ?? string.Empty));
	}
	
	[Authorize]
	[HttpPost(APIRoute.Authentication.RegenerateBackupCodeConfirm, Name = nameof(RegenerateBackupCodeConfirmAsync))]
	public async Task<IActionResult> RegenerateBackupCodeConfirmAsync([FromBody] RegenerateBackupConfirmRequest req)
	{
		// Retrieve user email from token
		var email = User.FindFirst(ClaimTypes.Email)?.Value;
		return Ok(await _userSvc.ConfirmRegenerateMfaBackupCodeAsync(email ?? string.Empty, req.Otp, req.Token));
	}

	[Authorize]
	[HttpGet(APIRoute.Authentication.GetMfaBackupAsync, Name = nameof(GetMfaBackupAsyncAsync))]
	public async Task<IActionResult> GetMfaBackupAsyncAsync()
	{
		// Retrieve user email from token
		var email = User.FindFirst(ClaimTypes.Email)?.Value;
		return Ok(await _userSvc.GetMfaBackupAsync(email ?? string.Empty));
	}

	[Authorize]
	[HttpPut(APIRoute.Authentication.UpdateProfile, Name = nameof(UpdateProfileAsync))]
	public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileRequest req)
	{
		// Retrieve user email & user type from token
		var email = User.FindFirst(ClaimTypes.Email)?.Value;
		return Ok(await _userSvc.UpdateProfileAsync(req.ToUserDto(email ?? string.Empty)));
	}
}