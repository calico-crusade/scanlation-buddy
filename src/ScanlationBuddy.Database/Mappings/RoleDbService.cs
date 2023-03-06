namespace ScanlationBuddy.Database.Mappings;

public interface IRoleDbService
{
	Task<int> RoleCount();

	Task<BuddyRole> Fetch(long id);

	Task Insert(BuddyRole role);

	Task Update(BuddyRole role);

	Task<BuddyRole[]> All();

	Task<string[]> Permissions(long userId);

	Task<BuddyRole[]> Roles(long userId);

	Task<bool> ToggleRole(long userId, long roleId, long creatorId);
}

public class RoleDbService : OrmMapExtended<BuddyRole>, IRoleDbService
{
	private const string TABLE_NAME_USER_ROLE = "buddy_user_role";
	private string? _insertUserRole;

	public override string TableName => "buddy_role";

	public RoleDbService(IDbQueryBuilderService query, ISqlService sql) : base(query, sql) { }

	public Task<int> RoleCount()
	{
		return _sql.ExecuteScalar<int>("SELECT COUNT(*) FROM buddy_role WHERE deleted_at IS NULL");
	}

	public Task<BuddyRole[]> Roles(long userId)
	{
		const string QUERY = @"SELECT 
	r.* 
FROM buddy_role r
JOIN buddy_user_role ur ON ur.role_id = r.id
JOIN buddy_user u ON u.id = ur.user_id
WHERE 
	ur.user_id = :userId AND
	r.deleted_at IS NULL AND
	ur.deleted_at IS NULL AND
	u.deleted_at IS NULL";
		return _sql.Get<BuddyRole>(QUERY, new { userId });
	}

	public async Task<string[]> Permissions(long userId)
	{
		var roles = await Roles(userId);
		return roles.SelectMany(t => t.Permissions).Distinct().ToArray();
	}

	public async Task<bool> ToggleRole(long userId, long roleId, long creatorId)
	{
		const string FETCH = "SELECT * FROM buddy_user_role WHERE user_id = :userId AND role_id = :roleId";
		const string DELETE = "DELETE FROM buddy_user_role WHERE id = :id";
		_insertUserRole ??= _query.Insert<BuddyUserRole>(TABLE_NAME_USER_ROLE);

		var context = new BuddyUserRole
		{
			RoleId = roleId,
			UserId = userId,
			CreatorId = creatorId
		};

		var exists = await _sql.Fetch<BuddyUserRole>(FETCH, new { userId, roleId });
		if (exists == null)
		{
			await _sql.Execute(_insertUserRole, context);
			return true;
		}

		await _sql.Execute(DELETE, new { id = exists.Id });
		return false;
	}
}
