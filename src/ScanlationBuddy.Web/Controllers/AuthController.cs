namespace ScanlationBuddy.Web.Controllers;

using Auth;
using Database;

[ApiController]
public class AuthController : ControllerBase
{
	private readonly IDbService _db;
	private readonly IOAuthService _auth;
	private readonly ITokenService _token;

	public AuthController(
		IDbService db, 
		IOAuthService auth,
		ITokenService token)
	{
		_db = db;
		_auth = auth;
		_token = token;
	}

	[HttpGet, Route("api/auth/{code}")]
	public async Task<IActionResult> Auth([FromRoute] string code)
	{
		var res = await _auth.ResolveCode(code);
		if (res == null || !string.IsNullOrEmpty(res.Error))
			return Unauthorized(new
			{
				error = res?.Error ?? "Login Failed"
			});

		var user = new BuddyUser
		{
			Avatar = res.User.Avatar,
			Email = res.User.Email,
			PlatformId = res.User.Id,
			Username = res.User.Nickname,
			Provider = res.User.Provider,
			ProviderId = res.User.ProviderId,
		};

		var id = await _db.Users.Upsert(user);
		user = await _db.Users.Fetch(res.User.Id);

		var roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
		var token = _token.GenerateToken(res, roles);

		return Ok(new {
			user = new
			{
				roles,
				nickname = res.User.Nickname,
				avatar = res.User.Avatar,
				id = res.User.Id,
				email = res.User.Email
			},
			token,
			id
		});
	}

	[HttpGet, Route("api/auth"), Authorize]
	public IActionResult Me()
	{
		var user = this.UserFromIdentity();
		if (user == null) return Unauthorized();

		var roles = User.Claims.Where(t => t.Type == ClaimTypes.Role).Select(t => t.Value).ToArray();

		return Ok(new
		{
			roles,
			nickname = user.Nickname,
			avatar = user.Avatar,
			id = user.Id,
			email = user.Email
		});
	}

	[HttpGet, Route("api/auth/is-first-time"), Authorize]
	public async Task<IActionResult> SetupFirstTime()
	{
		var user = this.UserFromIdentity();
		if (user == null) return Unauthorized();

		var profile = await _db.Users.Fetch(user.Id);
		if (profile == null) return NotFound();

		var count = await _db.Users.UserCount();
		if (count != 1) return BadRequest();

		await _db.Users.UpdateRoles(profile.Id, ROLE_ADMIN);

		return Ok(new { worked = true });
	}
}
