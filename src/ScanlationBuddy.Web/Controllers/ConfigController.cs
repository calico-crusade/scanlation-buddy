namespace ScanlationBuddy.Web.Controllers;

[ApiController, Authorize(Roles = PERM_ADMIN_CONFIG)]
public class ConfigController : ControllerBase
{
	private readonly IDbService _db;

	public ConfigController(IDbService db)
	{
		_db = db;
	}

	[HttpGet, Route("api/config")]
	public async Task<IActionResult> Get()
	{
		var data = await _db.Configs.All();
		return Ok(data);
	}

	[HttpPut, Route("api/config")]
	public async Task<IActionResult> Put([FromBody] BuddyConfig config)
	{
		await _db.Configs.Update(config);
		return Ok();
	}
}
