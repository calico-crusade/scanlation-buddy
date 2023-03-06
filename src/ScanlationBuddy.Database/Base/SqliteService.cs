using Dapper;
using System.Data;
using System.Data.SQLite;

namespace ScanlationBuddy.Database;

public class SqliteService : SqlService
{
	private readonly IConfiguration _config;
	private readonly ILogger _logger;

	public override int Timeout => int.TryParse(_config["Database:Timeout"], out int timeout) ? timeout : 0;
	public bool FirstRun { get; private set; } = true;

	public SqliteService(
		IConfiguration config,
		ILogger<SqliteService> logger)
	{
		_config = config;
		_logger = logger;
	}

	public override IDbConnection CreateConnection()
	{
		var conString = _config["Database:ConnectionString"];
		if (string.IsNullOrEmpty(conString))
			throw new ArgumentNullException("conString", "Connection string is not specified in the configuration");
		var con = new SQLiteConnection(conString);
		con.Open();

		if (!FirstRun) return con;

		FirstRun = false;
		ExecuteScripts(con).Wait();

		return con;
	}

	public async Task ExecuteScripts(IDbConnection con)
	{
		var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
		if (!Directory.Exists(path)) return;

		var files = Directory
			.GetFiles(path, "*.sql", SearchOption.AllDirectories)
			.OrderBy(t => Path.GetFileName(t))
			.ToArray();
		if (files.Length <= 0) return;

		foreach (var file in files)
		{
			try
			{
				_logger.LogInformation($"Executing Script: {file}");
				var content = await File.ReadAllTextAsync(file);
				await con.ExecuteAsync(content);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occurred while executing script: {file}");
			}
		}
	}
}
