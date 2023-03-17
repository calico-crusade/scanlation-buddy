namespace ScanlationBuddy.Database.Mappings;

public interface IConfigDbService
{
	Task<string?> Get(string key);

	Task Update(BuddyConfig config);

	Task Insert(BuddyConfig config);

	Task<BuddyConfig[]> All();

	Task Intialize();
}

public class ConfigDbService : OrmMapExtended<BuddyConfig>, IConfigDbService
{
	public override string TableName => "buddy_config";

	private CacheItem<BuddyConfig[]> _cache;

	public ConfigDbService(IDbQueryBuilderService query, ISqlService sql) : base(query, sql) 
	{
		_cache = new CacheItem<BuddyConfig[]>(base.All);
	}

	private void InvalidateCache()
	{
		_cache = new CacheItem<BuddyConfig[]>(base.All);
	}

	public async Task<string?> Get(string key)
	{
		return (await All())
			.FirstOrDefault(t => t.Key == key)?
			.Value;
	}

	public override async Task<BuddyConfig[]> All()
	{
		return await _cache.Get() ?? Array.Empty<BuddyConfig>();
	}

	public override Task Insert(BuddyConfig obj)
	{
		InvalidateCache();
		return base.Insert(obj);
	}

	public override Task Update(BuddyConfig obj)
	{
		InvalidateCache();
		return base.Update(obj);
	}

	public async Task Intialize()
	{
		var configs = await All();
		var defaults = BuddyConfig.DEFAULTS;

		foreach(var def in defaults)
		{
			if (configs.Any(t => t.Key == def.Key)) continue;

			await Insert(def);
		}

	}
}
