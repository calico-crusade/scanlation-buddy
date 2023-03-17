namespace ScanlationBuddy.Database;

using Mappings;

public interface IDbService
{
	IUserDbService Users { get; }

	IProjectDbService Projects { get; }

	IRoleDbService Roles { get; }

	IFileDbService Files { get; }

	IConfigDbService Configs { get; }
}

public class DbService : IDbService
{
	public IUserDbService Users { get; }

	public IProjectDbService Projects { get; }

	public IRoleDbService Roles { get; }

	public IFileDbService Files { get; }

	public IConfigDbService Configs { get; }

	public DbService(
		IUserDbService users, 
		IProjectDbService projects,
		IRoleDbService roles,
		IConfigDbService configs,
		IFileDbService files)
	{
		Users = users;
		Projects = projects;
		Roles = roles;
		Configs = configs;
		Files = files;
	}
}
