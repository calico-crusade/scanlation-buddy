using System;

namespace ScanlationBuddy.Database.Mappings;

public interface IUserDbService
{
	Task<(long id, bool isNew)> Upsert(BuddyUser user);

	Task<BuddyUser?> Fetch(long userId);

	Task<BuddyUser?> Fetch(string platformId);

	Task<int> UserCount();

	Task<PaginatedResult<BuddyUser>> Paginate(int page = 1, int size = 100);

	Task<PaginatedResult<BuddyUserRoles>> UsersRoles(UserFilters filters);

	Task<string[]> Providers();
}

public class UserDbService : OrmMapExtended<BuddyUser>, IUserDbService
{
	private static string? _getQuery;

	public UserDbService(IQueryService query, ISqlService sql) : base(query, sql) { }

	public Task<int> UserCount()
	{
		return _sql.ExecuteScalar<int>("SELECT COUNT(*) FROM buddy_user WHERE deleted_at IS NULL");
	}

	public Task<(long id, bool isNew)> Upsert(BuddyUser user)
	{
		return Upsert(user, v => v.With(t => t.PlatformId));
	}
	
	public Task<BuddyUser?> Fetch(string platformId)
	{
		_getQuery ??= _query.Select<BuddyUser>(t => t.With(t => t.PlatformId));

		return _sql.Fetch<BuddyUser>(_getQuery, new { PlatformId = platformId });
	}

	public async Task<PaginatedResult<BuddyUserRoles>> UsersRoles(UserFilters filters)
	{
		const string QUERY = @"SELECT
	DISTINCT
    u.*
FROM buddy_user u
LEFT JOIN buddy_user_role bur ON u.id = bur.user_id AND bur.deleted_at IS NULL
LEFT JOIN buddy_role r ON r.id = bur.role_id AND r.deleted_at IS NULL
WHERE
    {0}
ORDER BY u.created_at DESC
LIMIT :limit
OFFSET :offset;

SELECT
	COUNT(DISTINCT u.id)
FROM buddy_user u
LEFT JOIN buddy_user_role bur ON u.id = bur.user_id AND bur.deleted_at IS NULL
LEFT JOIN buddy_role r ON r.id = bur.role_id AND r.deleted_at IS NULL
WHERE
    {0};

SELECT
	DISTINCT
    r.*,
	bur.user_id as user_id
FROM buddy_role r
JOIN buddy_user_role bur on r.id = bur.role_id AND bur.deleted_at IS NULL
JOIN buddy_user u ON u.id = bur.user_id AND u.deleted_at IS NULL
WHERE
	r.deleted_at IS NULL AND
    bur.user_id IN (
        SELECT
			u.id
		FROM buddy_user u
		JOIN buddy_user_role bur ON u.id = bur.user_id AND bur.deleted_at IS NULL
		JOIN buddy_role r ON r.id = bur.role_id AND r.deleted_at IS NULL
		WHERE
			{0}
    )
ORDER BY u.created_at DESC;";

		var parts = new List<string>
		{
			"u.deleted_at IS NULL"
		};
		var pars = new DynamicParameters();
		pars.Add("offset", (filters.Page - 1) * filters.Size);
		pars.Add("limit", filters.Size);

		if (!string.IsNullOrEmpty(filters.Username))
		{
			pars.Add("filter", $"%{filters.Username.ToUpper().Trim()}%");
			parts.Add("UPPER(u.username) LIKE :filter");
		}

		if (!string.IsNullOrEmpty(filters.Provider))
		{
			pars.Add("provider", filters.Provider);
			parts.Add("u.provider = :provider");
		}

		if (filters.RoleId != null)
		{
			pars.Add("roleId", filters.RoleId);
			parts.Add("r.id = :roleId");
		}

		var query = string.Format(QUERY, string.Join(" AND ", parts));

		using var con = await _sql.CreateConnection();
		using var rdr = await con.QueryMultipleAsync(query, pars);

		var users = (await rdr.ReadAsync<BuddyUser>()).ToArray();
		var total = await rdr.ReadSingleAsync<int>();
		var roles = (await rdr.ReadAsync<BuddyRoleUser>()).ToArray();

		var results = MapUsersRoles(users, roles).ToArray();
		var pages = (int)Math.Ceiling((double)total / filters.Size);
		return new PaginatedResult<BuddyUserRoles>(pages, total, results);
	}

	public IEnumerable<BuddyUserRoles> MapUsersRoles(BuddyUser[] users, BuddyRoleUser[] roles)
	{
		BuddyUserRoles? current = null;
		for(int u = 0, r = 0; u < users.Length;)
		{
			var user = users[u];
			if (current == null || user.Id != current.User.Id)
			{
				if (current != null) yield return current;
				current = new BuddyUserRoles { User = user };
			}

			if (r >= roles.Length)
			{
				u++;
				continue;
			};

			var role = roles[r];
			if (user.Id != role.UserId)
			{
				u++;
				continue;
			}

			current.Roles.Add(role);
			r++;
		}

		if (current != null) yield return current;
	}

	public Task<string[]> Providers()
	{
		return _sql.Get<string>("SELECT DISTINCT provider FROM buddy_user");
	}
}

public class UserFilters
{
	[JsonPropertyName("page")]
	public int Page { get; set; } = 1;

	[JsonPropertyName("size")]
	public int Size { get; set; } = 100;

	[JsonPropertyName("username")]
	public string? Username { get; set; }

	[JsonPropertyName("provider")]
	public string? Provider { get; set; }

	[JsonPropertyName("roleId")]
	public long? RoleId { get; set; }
}