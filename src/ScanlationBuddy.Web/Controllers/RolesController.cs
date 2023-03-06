namespace ScanlationBuddy.Web.Controllers;

[ApiController]
public class RolesController : ControllerBase
{
	private readonly IDbService _db;

	public RolesController(IDbService db)
	{
		_db = db;
	}

	[HttpGet, Route("api/roles/permissions")]
	[ProducesDefaultResponseType(typeof(Permission[]))]
	public IActionResult Permissions()
	{
		return Ok(PermissionDescriptions);
	}

	[HttpGet, Route("api/roles")]
	[ProducesDefaultResponseType(typeof(BuddyRole[]))]
	public async Task<IActionResult> Get()
	{
		var roles = await _db.Roles.All();
		return Ok(roles);
	}

	[HttpPost, Route("api/roles"), Authorize(Roles = PERM_ADMIN_EDIT_ROLES)]
	public async Task<IActionResult> Post([FromBody] BuddyRole role)
	{
		var id = this.Id();
		if (id == null) return Unauthorized();

		role.CreatorId = id.Value;
		await _db.Roles.Insert(role);
		return Ok();
	}

	[HttpPut, Route("api/roles"), Authorize(Roles = PERM_ADMIN_EDIT_ROLES)]
	public async Task<IActionResult> Put([FromBody] BuddyRole role)
	{
		await _db.Roles.Update(role);
		return Ok();
	}

	[HttpDelete, Route("api/roles/{id}"), Authorize(Roles = PERM_ADMIN_EDIT_ROLES)]
	public async Task<IActionResult> Delete([FromRoute] long id)
	{
		var role = await _db.Roles.Fetch(id);
		if (role == null) return NotFound();

		role.DeletedAt = DateTime.Now;
		await _db.Roles.Update(role);
		return Ok();
	}
}
