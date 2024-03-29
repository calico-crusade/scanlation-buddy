﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ScanlationBuddy.Auth;

public interface ITokenService
{
	TokenResult ParseToken(string token);
	string GenerateToken(Action<JwtToken> config);
	string GenerateToken(TokenResponse resp, params string[] roles);
}

public class TokenService : ITokenService
{
	private readonly IConfiguration _config;

	public TokenService(IConfiguration config)
	{
		_config = config;
	}

	public SymmetricSecurityKey GetKey()
	{
		return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["OAuth:Key"]));
	}

	public TokenValidationParameters GetParameters()
	{
		return new TokenValidationParameters
		{
			IssuerSigningKey = GetKey(),
			ValidateAudience = true,
			ValidateIssuer = true,
			ValidateIssuerSigningKey = true,
			ValidAudience = _config["OAuth:Audience"],
			ValidIssuer = _config["OAuth:Issuer"],
		};
	}

	public TokenResult ParseToken(string token)
	{
		var validationParams = GetParameters();

		var handler = new JwtSecurityTokenHandler();

		var principals = handler.ValidateToken(token, validationParams, out var securityToken);

		return new(principals, securityToken);
	}

	public string GenerateToken(TokenResponse resp, params string[] roles)
	{
		return GenerateToken(t =>
		{
			t.AddClaim(ClaimTypes.NameIdentifier, resp.User.Id)
			.AddClaim(ClaimTypes.Name, resp.User.Nickname)
			.AddClaim(ClaimTypes.Email, resp.User.Email)
			.AddClaim(ClaimTypes.UserData, resp.User.Avatar)
			.AddClaim(ClaimTypes.PrimarySid, resp.Provider)
			.AddClaim(ClaimTypes.PrimaryGroupSid, resp.User.ProviderId)
			.AddClaim(roles.Select(t => new Claim(ClaimTypes.Role, t)).ToArray());
		});
	}

	public string GenerateToken(Action<JwtToken> config)
	{
		int? expires = int.TryParse(_config["OAuth:ExpiryMinutes"], out int mins) ? mins : null;
		var token = new JwtToken(GetKey())
			.SetAudience(_config["OAuth:Audience"])
			.SetIssuer(_config["OAuth:Issuer"])
			.Expires(expires);

		config?.Invoke(token);

		return token.Write();
	}
}

public record class TokenResult(ClaimsPrincipal Principal, SecurityToken Token);
