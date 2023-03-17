namespace ScanlationBuddy.Web.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
	private readonly IDbService _db;
	private readonly IOAuthService _auth;
	private readonly ITokenService _token;
	private readonly IUserService _user;

	public AuthController(
		IDbService db, 
		IOAuthService auth,
		ITokenService token,
		IUserService user)
	{
		_db = db;
		_auth = auth;
		_token = token;
		_user = user;
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

		var token = await _user.Login(res);
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
}
