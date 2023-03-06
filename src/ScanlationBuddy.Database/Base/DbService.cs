namespace ScanlationBuddy.Database;

using Mappings;

public interface IDbService
{
	IUserDbService Users { get; }

	IProjectDbService Projects { get; }

	IRoleDbService Roles { get; }
}

public class DbService : IDbService
{
	public IUserDbService Users { get; }

	public IProjectDbService Projects { get; }

	public	IRoleDbService Roles { get; }

	public DbService(
		IUserDbService users, 
		IProjectDbService projects,
		IRoleDbService roles)
	{
		Users = users;
		Projects = projects;
		Roles = roles;
	}
}
