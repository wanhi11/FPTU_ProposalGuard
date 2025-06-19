using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Application.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace FPTU_ProposalGuard.Application.Utils
{
    //	Summary:
    //		This class is to provide procedures in order to generate JWT token
    public class JwtUtils
    {
	    private readonly WebTokenSettings _webTokenSettings;
	    private readonly TokenValidationParameters _tokenValidationParameters;

	    public JwtUtils() 
		{
			_webTokenSettings = null!;
			_tokenValidationParameters = null!;
		}
	    
	    public JwtUtils(
		    WebTokenSettings webTokenSettings)
	    {
		    _webTokenSettings = webTokenSettings;
			_tokenValidationParameters = null!;
		}
		
	    public JwtUtils(
		    TokenValidationParameters tokenValidationParameters)
	    {
			_webTokenSettings = null!;
			_tokenValidationParameters = tokenValidationParameters;
	    }
	    
	    public JwtUtils(
			TokenValidationParameters tokenValidationParameters,
			WebTokenSettings webTokenSettings)
		{
			_webTokenSettings = webTokenSettings;
			_tokenValidationParameters = tokenValidationParameters;
		}

		// Generate JWT token 
		public async Task<(string AccessToken, DateTime ValidTo)> GenerateJwtTokenAsync(
			string tokenId, UserDto user)
		{
			// Get secret key
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_webTokenSettings.IssuerSigningKey));
		
			// Jwt Handler
			var jwtTokenHandler = new JwtSecurityTokenHandler();
		
			// Token claims 
			List<Claim> authClaims = new()
			{
				new Claim(ClaimTypes.Role, user.Role.RoleName),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}".Trim()),
				new Claim(JwtRegisteredClaimNames.Jti, tokenId),
			};
		
			// Token descriptor 
			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				// Token claims (email, role, username, id...)
				Subject = new ClaimsIdentity(authClaims),
				Expires = DateTime.UtcNow.AddMinutes(_webTokenSettings.TokenLifeTimeInMinutes),
				Issuer = _webTokenSettings.ValidIssuer,
				Audience = _webTokenSettings.ValidAudience,
				SigningCredentials = new SigningCredentials(
					authSigningKey, SecurityAlgorithms.HmacSha256)
			};
		
			// Generate token with descriptor
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			return await Task.FromResult((jwtTokenHandler.WriteToken(token), token.ValidTo));
		}
		
		// Generate password reset token
		public async Task<string> GeneratePasswordResetTokenAsync(UserDto user)
		{
			// Get secret key
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_webTokenSettings.IssuerSigningKey));
		
			// Jwt Handler
			var jwtTokenHandler = new JwtSecurityTokenHandler();
		
			// Token claims 
			List<Claim> authClaims = new()
			{
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
		
			// Token descriptor 
			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				// Token claims (email, role, username, id...)
				Subject = new ClaimsIdentity(authClaims),
				Expires = DateTime.UtcNow.AddMinutes(_webTokenSettings.RecoveryPasswordLifeTimeInMinutes),
				Issuer = _webTokenSettings.ValidIssuer,
				Audience = _webTokenSettings.ValidAudience,
				SigningCredentials = new SigningCredentials(
					authSigningKey, SecurityAlgorithms.HmacSha256)
			};
		
			// Generate token with descriptor
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			return await Task.FromResult(jwtTokenHandler.WriteToken(token));
		}
		
		// Generate MFA token
		public async Task<string> GenerateMfaTokenAsync(UserDto user)
		{
			// Get secret key
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_webTokenSettings.IssuerSigningKey));
		
			// Jwt Handler
			var jwtTokenHandler = new JwtSecurityTokenHandler();
		
			// Token claims 
			List<Claim> authClaims = new()
			{
				new Claim(CustomClaimTypes.Mfa, ClaimValues.MFA_CLAIMVALUE),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
		
			// Token descriptor 
			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				// Token claims (email, role, username, id...)
				Subject = new ClaimsIdentity(authClaims),
				Expires = DateTime.UtcNow.AddMinutes(_webTokenSettings.MfaTokenLifeTimeInMinutes),
				Issuer = _webTokenSettings.ValidIssuer,
				Audience = _webTokenSettings.ValidAudience,
				SigningCredentials = new SigningCredentials(
					authSigningKey, SecurityAlgorithms.HmacSha256)
			};
		
			// Generate token with descriptor
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			return await Task.FromResult(jwtTokenHandler.WriteToken(token));
		}
		
		// Validate MFA token
		public async Task<JwtSecurityToken?> ValidateMfaTokenAsync(string token)
		{
			// Initialize token handler
			var tokenHandler = new JwtSecurityTokenHandler();
		
			// Check if the token format is valid
			if (!tokenHandler.CanReadToken(token))
				throw new UnauthorizedException("Invalid token format.");
		
			try
			{
				// Validate token
				var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
		
				// Ensure the token is a JWT
				if (validatedToken is not JwtSecurityToken jwtToken)
					throw new UnauthorizedException("Invalid token type.");
		
				// Ensure the algorithm used matches the expected algorithm
				if (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
					throw new UnauthorizedException("Invalid token algorithm.");
		
				// Check for MFA claim
				var mfaClaim = principal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Mfa);
				if (mfaClaim == null || mfaClaim.Value != ClaimValues.MFA_CLAIMVALUE)
					throw new UnauthorizedException("MFA claim is missing or invalid.");
				
				// Mark as complete
				await Task.CompletedTask;
				// Mark as valid token
				return jwtToken;
			}
			catch (SecurityTokenException ex)
			{
				// Handle token validation errors
				throw new UnauthorizedException("Token validation failed: " + ex.Message);
			}
		}
		
		// Generate refresh token
		public async Task<string> GenerateRefreshTokenAsync()
		{
			var randomNumber = new byte[64];
		
			using (var numberGenerator = RandomNumberGenerator.Create())
			{
				numberGenerator.GetBytes(randomNumber);
			}
		
			return await Task.FromResult(Convert.ToBase64String(randomNumber));
		}
		
		// Validate access token
		public async Task<JwtSecurityToken?> ValidateAccessTokenAsync(string token)
		{
			// Initialize token handler
			var tokenHandler = new JwtSecurityTokenHandler();
		
			// Check if the token format is valid
			if (!tokenHandler.CanReadToken(token))
				throw new UnauthorizedException("Invalid token format.");
		
			// Validate token
			var validationResult = await tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters);
			if (!validationResult.IsValid)
				throw new UnauthorizedException("Token validation failed.");
		
			// Converts a string into an instance of JwtSecurityToken
			return tokenHandler.ReadJwtToken(token) ?? null;
		}
		
		// Validate access token
		public JwtSecurityToken? ValidateExpiredAccessToken(string token)
		{
			// Initialize token handler
			var tokenHandler = new JwtSecurityTokenHandler();
			
			try
			{
				// Check if the token format is valid
				if (!tokenHandler.CanReadToken(token))
					throw new UnauthorizedException("Invalid token format.");
		
				// Validate access token
				tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
				
				return (JwtSecurityToken?) validatedToken;
			}
			catch (SecurityTokenExpiredException)
			{
				// Converts a string into an instance of JwtSecurityToken
				var decryptedToken = tokenHandler.ReadJwtToken(token) ?? null;
				return decryptedToken;
			}
			catch (Exception)
			{
				return null;
			}
		}
    }
}
