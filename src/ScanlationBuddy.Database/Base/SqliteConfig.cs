using Microsoft.Data.Sqlite;

namespace ScanlationBuddy.Database;

public class SqliteConfig : ISqlConfig<SqliteConnection>
{
	private readonly IConfiguration _config;

	public int Timeout => int.TryParse(_config["Database:Timeout"], out var timeout) ? timeout : 0;

	public string ConnectionString => _config["Database:ConnectionString"];

	public SqliteConfig(IConfiguration config)
	{
		_config = config;
	}
}
