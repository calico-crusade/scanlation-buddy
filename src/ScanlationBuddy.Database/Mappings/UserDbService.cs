namespace ScanlationBuddy.Database.Mappings;

public interface IUserDbService
{
	Task<long> Upsert(BuddyUser user);

	Task<BuddyUser> Fetch(string platformId);

	Task UpdateRoles(long id, string roles);

	Task<int> UserCount();
}

public class UserDbService : OrmMapExtended<BuddyUser>, IUserDbService
{
	private string? _getQuery;
	public override string TableName => "buddy_user";

	public UserDbService(IDbQueryBuilderService query, ISqlService sql) : base(query, sql) { }

	public Task<int> UserCount()
	{
		return _sql.ExecuteScalar<int>("SELECT COUNT(*) FROM buddy_user WHERE deleted_at IS NULL");
	}

	public Task<long> Upsert(BuddyUser user)
	{
		return Upsert(user, 
			v => v.With(t => t.PlatformId),
			updates: v => v.With(t => t.Id).With(t => t.CreatedAt).With(t => t.Roles)
		);
	}

	public Task<BuddyUser> Fetch(string platformId)
	{
		_getQuery ??= _query.Select<BuddyUser>(TableName, t => t.With(t => t.PlatformId));

		return _sql.Fetch<BuddyUser>(_getQuery, new { platformId });
	}

	public Task UpdateRoles(long id, string roles)
	{
		const string QUERY = "UPDATE buddy_user SET roles = :roles WHERE id = :id";
		return _sql.Execute(QUERY, new { id, roles });
	}
}
