namespace ScanlationBuddy.Web.Controllers;

using Database.Mappings;

[ApiController, Authorize(Roles = PERM_ACCESS_SITE)]
public class UserController : ControllerBase
{
	private readonly IDbService _db;

	public UserController(IDbService db)
	{
		_db = db;
	}

	[HttpGet, Route("api/users")]
	[ProducesDefaultResponseType(typeof(PaginatedResult<BuddyUser>))]
	public async Task<IActionResult> Get(
		[FromQuery] int page = 1, 
		[FromQuery] int size = 100,
		[FromQuery] string? username = null,
		[FromQuery] string? provider = null,
		[FromQuery] long? roleId = null)
	{
		var filter = new UserFilters
		{
			Page = page,
			Size = size,
			Username = username,
			Provider = provider,
			RoleId = roleId
		};

		var users = await _db.Users.UsersRoles(filter);
		return Ok(users);
	}

	[HttpPut, Route("api/users/{userId}/roles"), Authorize(Roles = PERM_ADMIN_EDIT_ROLES)]
	public async Task<IActionResult> UpdateRoles([FromRoute] long userId, [FromBody] long[] roles)
	{
		var id = this.Id();
		if (id == null) return Unauthorized();

		var userRoles = (await _db.Roles.Roles(userId)).Select(t => t.Id).ToArray();
		var toggle = userRoles.Except(roles)
			.Union(roles.Except(userRoles))
			.ToArray();

		foreach(var role in toggle)
			await _db.Roles.ToggleRole(userId, role, id.Value);

		return Ok();
	}

	[HttpGet, Route("api/users/providers")]
	[ProducesDefaultResponseType(typeof(string[]))]
	public async Task<IActionResult> Providers()
	{
		return Ok(await _db.Users.Providers());
	}
}
