namespace ScanlationBuddy.Web.Controllers;

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

		await _db.Users.Upsert(user);
		user = await _db.Users.Fetch(res.User.Id);

		var token = _token.GenerateToken(t =>
		{
			t[ClaimTypes.NameIdentifier] = user.Id.ToString();
		});

		return Ok(new { token });
	}

	[HttpGet, Route("api/auth"), Authorize]
	public async Task<IActionResult> Me()
	{
		var id = this.Id();
		if (id == null) return Unauthorized();

		var profile = await _db.Users.Fetch(id.Value);
		if (profile == null) return Unauthorized();

		var roles = await _db.Roles.Roles(profile.Id);

		return Ok(new
		{
			roles,
			profile
		});
	}

	[HttpGet, Route("api/auth/is-first-time"), Authorize]
	public async Task<IActionResult> SetupFirstTime()
	{
		var id = this.Id();
		if (id == null) return Unauthorized();

		var profile = await _db.Users.Fetch(id.Value);
		if (profile == null) return NotFound();

		var userCount = await _db.Users.UserCount();
		if (userCount != 1) return BadRequest();

		var roleCount = await _db.Roles.RoleCount();
		if (roleCount <= 0)
			await _db.Roles.Insert(new BuddyRole
			{
				Name = "Site Owner",
				Description = "The overlord of the application. All shall bow down before this user's majesty.",
				Permissions = PERMS_ALL,
				Color = "#726ae4",
				BadgeId = null,
				CreatorId = profile.Id
			});

		var roles = (await _db.Roles.All()).Select(t => t.Id);
		foreach (var role in roles)
			await _db.Roles.ToggleRole(profile.Id, role, profile.Id);

		return Ok(new { worked = true });
	}
}
