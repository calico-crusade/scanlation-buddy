using System.Security.Claims;

namespace ScanlationBuddy.Core;

public interface IUserService
{
	Task<string> Login(TokenResponse res);
}

public class UserService : IUserService
{
	private readonly IDbService _db;
	private readonly ITokenService _token;

	public UserService(
		IDbService db, 
		ITokenService token)
	{
		_db = db;
		_token = token;
	}

	public async Task<string> Login(TokenResponse res)
	{
		var (id, isNew) = await _db.Users.Upsert(new BuddyUser
		{
			Avatar = res.User.Avatar,
			Email = res.User.Email,
			PlatformId = res.User.Id,
			Username = res.User.Nickname,
			Provider = res.User.Provider,
			ProviderId = res.User.ProviderId,
		});

		if (isNew) await HandleNewUser(id);

		var token = _token.GenerateToken(t =>
		{
			t[ClaimTypes.NameIdentifier] = id.ToString();
		});

		return token;
	}

	public async Task HandleNewUser(long id)
	{
		await HandleFirstTimeCheck(id);
		await HandleDefaultRoles(id);
	}

	public async Task HandleDefaultRoles(long id)
	{
		var strRoles = await _db.Configs.Get(CONFIG_DEFAULT_ROLES);
		if (string.IsNullOrEmpty(strRoles)) return;

		var roles = strRoles.Split(new[] { ',', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
			.Select(t => (long.TryParse(t.Trim(), out var val), val))
			.Where(t => t.Item1)
			.Select(t => t.val)
			.ToArray();

		foreach (var role in roles)
			await _db.Roles.ToggleRole(id, role, id);
	}

	public async Task HandleFirstTimeCheck(long id)
	{
		var userCount = await _db.Users.UserCount();
		if (userCount != 1) return;

		await _db.Configs.Intialize();

		var roleCount = await _db.Roles.RoleCount();
		if (roleCount <= 0)
			await _db.Roles.Insert(new BuddyRole
			{
				Name = "Site Owner",
				Description = "The overlord of the application. All shall bow down before this user's majesty.",
				Permissions = PERMS_ALL,
				Color = "#726ae4",
				BadgeId = null,
				CreatorId = id
			});

		var roles = (await _db.Roles.All()).Select(t => t.Id);
		foreach (var role in roles)
			await _db.Roles.ToggleRole(id, role, id);
	}
}