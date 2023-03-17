using Dapper;

namespace ScanlationBuddy.Database;

public abstract class OrmMap<T>
{
	private string? _allQuery;
	private string? _allNonDeletedQuery;

	public readonly IDbQueryBuilderService _query;
	public readonly ISqlService _sql;

	public abstract string TableName { get; }

	public OrmMap(
		IDbQueryBuilderService query,
		ISqlService sql)
	{
		_query = query;
		_sql = sql;
	}

	public virtual Task<T[]> All()
	{
		_allNonDeletedQuery ??= _query.SelectAllNonDeleted(TableName);
		return _sql.Get<T>(_allNonDeletedQuery);
	}

	public virtual Task<T[]> AllWithDeleted()
	{
		_allQuery ??= _query.SelectAll(TableName);
		return _sql.Get<T>(_allQuery);
	}
}

public abstract class OrmMapExtended<T> : OrmMap<T>
	where T : DbObject
{
	private string? _fetchQuery;
	private string? _insertQuery;
	private string? _updateQuery;
	private string? _paginateQuery;
	private string? _insertReturnQuery;

	private List<string> _queryCache = new();

	public OrmMapExtended(
		IDbQueryBuilderService query,
		ISqlService sql) : base(query, sql) { }

	public virtual Task<T> Fetch(long id)
	{
		_fetchQuery ??= _query.SelectId<T>(TableName);
		return _sql.Fetch<T>(_fetchQuery, new { id });
	}

	public virtual Task<long> InsertReturn(T obj)
	{
		_insertReturnQuery ??= _query.InsertReturn<T, long>(TableName, t => t.Id);
		return _sql.ExecuteScalar<long>(_insertReturnQuery, obj);
	}

	public virtual Task Insert(T obj)
	{
		_insertQuery ??= _query.Insert<T>(TableName);
		return _sql.Execute(_insertQuery, obj);
	}

	public virtual Task Update(T obj)
	{
		_updateQuery ??= _query.Update<T>(TableName);
		return _sql.Execute(_updateQuery, obj);
	}

	public virtual async Task<PaginatedResult<T>> Paginate(string query, object? pars = null, int page = 1, int size = 100)
	{
		var p = new DynamicParameters(pars);
		p.Add("offset", (page - 1) * size);
		p.Add("size", size);

		using var con = _sql.CreateConnection();
		using var rdr = await con.QueryMultipleAsync(query, p);

		var res = (await rdr.ReadAsync<T>()).ToArray();
		var total = await rdr.ReadSingleAsync<long>();

		var pages = (long)Math.Ceiling((double)total / size);
		return new PaginatedResult<T>(pages, total, res);
	}

	public virtual Task<PaginatedResult<T>> Paginate(int page = 1, int size = 100)
	{
		_paginateQuery ??= _query.Pagniate<T, DateTime?>(TableName, (c) => { }, t => t.UpdatedAt);
		return Paginate(_paginateQuery, null, page, size);
	}

	public async Task<(long id, bool isNew)> Upsert(T item, 
		Action<PropertyExpressionBuilder<T>> conflicts,
		Action<PropertyExpressionBuilder<T>>? inserts = null,
		Action<PropertyExpressionBuilder<T>>? updates = null,
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
			queryCache.Add(_query.Update(TableName, updates));
			queryCache.Add(_query.InsertReturn(TableName, v => v.Id, inserts));
			queryCache.Add(_query.Select(TableName, conflicts));
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
