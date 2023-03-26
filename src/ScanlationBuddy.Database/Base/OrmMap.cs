using Dapper;

namespace ScanlationBuddy.Database;

public abstract class OrmMap<T>
{
	private static string? _allQuery;
	private static string? _fetchQuery;
	private static string? _insertQuery;
	private static string? _updateQuery;

	public readonly IQueryService _query;
	public readonly ISqlService _sql;

	public OrmMap(
		IQueryService query,
		ISqlService sql)
	{
		_query = query;
		_sql = sql;
	}

	public virtual Task<T[]> AllWithDeleted()
	{
		_allQuery ??= _query.Select<T>();
		return _sql.Get<T>(_allQuery);
	}

	public virtual Task<T?> Fetch(long id)
	{
		_fetchQuery ??= _query.Fetch<T>();
		return _sql.Fetch<T>(_fetchQuery, new { Id = id });
	}

	public virtual Task Insert(T obj)
	{
		_insertQuery ??= _query.Insert<T>();
		return _sql.Execute(_insertQuery, obj);
	}

	public virtual Task Update(T obj)
	{
		_updateQuery ??= _query.Update<T>();
		return _sql.Execute(_updateQuery, obj);
	}

	public virtual Task<PaginatedResult<T>> Paginate(string query, object? pars = null, int page = 1, int size = 100)
	{
		return _sql.Paginate<T>(query, pars, page, size);
	}
}

public abstract class OrmMapExtended<T> : OrmMap<T>
	where T : DbObject
{
	private static string? _paginateQuery;
	private static string? _allNonDeletedQuery;
	private static List<string> _queryCache = new();

	public OrmMapExtended(
		IQueryService query,
		ISqlService sql) : base(query, sql) { }

	public virtual Task<T[]> All()
	{
		_allNonDeletedQuery ??= _query.Select<T>(t => t.Null(a => a.DeletedAt));
		return _sql.Get<T>(_allNonDeletedQuery);
	}

	public virtual Task<PaginatedResult<T>> Paginate(int page = 1, int size = 100)
	{
		_paginateQuery ??= _query.Paginate<T, DateTime?>(t => t.UpdatedAt);
		return Paginate(_paginateQuery, null, page, size);
	}

	public async Task<(long id, bool isNew)> Upsert(T item, 
		Action<IExpressionBuilder<T>> conflicts,
		Action<IExpressionBuilder<T>>? inserts = null,
		Action<IExpressionBuilder<T>>? updates = null,
		List<string>? cache = null)
	{
		inserts ??= (v) => v.With(t => t.Id);
		updates ??= (v) => v.With(t => t.Id).With(t => t.CreatedAt);
		var queryCache = cache ?? _queryCache;

		//Note: This is purely to combat the issue of postgres SERIAL and BIGSERIAL 
		//		primary keys incrementing even if it was an update was preformed
		//		because the record already exists
		if (queryCache.Count != 3)
		{
			queryCache.Clear();
			queryCache.Add(_query.Update(updates));
			queryCache.Add(_query.Insert<T>() + " RETURNING id");
			queryCache.Add(_query.Select(conflicts));
		}

		string update = _queryCache[0], insert = _queryCache[1], select = _queryCache[2];

		var exists = await _sql.Fetch<T>(select, item);
		if (exists == null)
			return (await _sql.ExecuteScalar<long>(insert, item), true);

		item.Id = exists.Id;
		await _sql.Execute(update, item);
		return (exists.Id, false);
	}
}
