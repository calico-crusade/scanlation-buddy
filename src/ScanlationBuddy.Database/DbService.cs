namespace ScanlationBuddy.Database;

using Mappings;

public interface IDbService
{
	IUserDbService Users { get; }
}

public class DbService : IDbService
{
	public IUserDbService Users { get; }

	public DbService(IUserDbService users)
	{
		Users = users;
	}
}
