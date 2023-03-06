using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ScanlationBuddy.Auth;

public static class Extensions
{
	public static IServiceCollection AddOAuth(this IServiceCollection services, IConfiguration config)
	{
		services
			.AddTransient<ITokenService, TokenService>()
			.AddTransient<IOAuthService, OAuthService>()
			.AddAuthentication(opt =>
			{
				opt.DefaultScheme = AuthMiddleware.SCHEMA;
			})
			.AddScheme<AuthMiddlewareOptions, AuthMiddleware>(AuthMiddleware.SCHEMA, c => { });
		return services;
	}

	public static string? GroupId(this ClaimsPrincipal principal) => principal.Claim(ClaimTypes.GroupSid);

	public static string? GroupId(this ControllerBase ctrl) => ctrl?.User?.GroupId();

	public static string? Claim(this ClaimsPrincipal principal, string claim)
	{
		return principal?.FindFirst(claim)?.Value;
	}

	public static string? Claim(this ControllerBase ctrl, string claim)
	{
		if (ctrl.User == null) return null;
		return ctrl.User.Claim(claim);
	}

	public static long? Id(this ControllerBase ctrl)
	{
		if (ctrl.User == null) return null;

		return ctrl.User.Id();
	}

	public static long? Id(this ClaimsPrincipal prin)
	{
		if (prin == null) return null;

		var getClaim = (string key) => prin.Claim(key) ?? "";

		var strId = getClaim(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(strId) ||
			!long.TryParse(strId, out var id)) return null;

		return id;
	}
}