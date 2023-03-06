using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ScanlationBuddy.Auth;

using Database;

public class AuthMiddlewareOptions : AuthenticationSchemeOptions { }

public class AuthMiddleware : AuthenticationHandler<AuthMiddlewareOptions>
{
	public const string SCHEMA = "scanlation-buddy-schema";

	private readonly IDbService _db;
	private readonly ITokenService _token;
	private readonly ILogger _logger;

	public AuthMiddleware(
		IOptionsMonitor<AuthMiddlewareOptions> options, 
		ILoggerFactory factory,
		ILogger<AuthMiddleware> logger,
		UrlEncoder encoder,
		ISystemClock clock,
		ITokenService token,
		IDbService db) : base(options, factory, encoder, clock)
	{
		_token = token;
		_db = db;
		_logger = logger;
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		try
		{
			var header = GetHeader("authorization", "access_token");
			if (string.IsNullOrEmpty(header))
				return AuthenticateResult.Fail("Authorization header not found.");

			if (header.ToLower().StartsWith("bearer "))
				header = header.Remove(0, "bearer ".Length).Trim();

			var (principal, token) = _token.ParseToken(header);

			var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(idClaim) || !long.TryParse(idClaim, out var id))
				return AuthenticateResult.Fail("Invalid Profile ID");

			var profile = await _db.Users.Fetch(id);
			if (profile == null) return AuthenticateResult.Fail("Profile doesn't exist");

			var permissions = await _db.Roles.Permissions(id) ?? Array.Empty<string>();
			var claims = permissions.Select(t => new Claim(ClaimTypes.Role, t));

			var identity = new ClaimsIdentity(SCHEMA);
			identity.AddClaims(claims);
			identity.AddClaim(new (ClaimTypes.NameIdentifier, id.ToString()));

			var prin = new ClaimsPrincipal();
			prin.AddIdentity(identity);

			return AuthenticateResult.Success(new AuthenticationTicket(prin, SCHEMA));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while validating JWT token");
			return AuthenticateResult.Fail(ex);
		}
	}

	public string? GetHeader(string key, string? queryVersion = null)
	{
		foreach (var header in Request.Headers)
			if (header.Key.ToLower() == key.ToLower())
				return header.Value;

		if (string.IsNullOrEmpty(queryVersion))
			return null;

		foreach (var query in Request.Query)
			if (query.Key.ToLower() == queryVersion.ToLower() ||
				query.Key.ToLower() == "access_token")
				return query.Value;

		return null;
	}
}
