namespace ScanlationBuddy.Database.Mappings;

public interface IFileDbService
{
	Task Insert(BuddyFile file);

	Task Update(BuddyFile file);

	Task<BuddyFile?> Fetch(long fileId);

	Task<PaginatedResult<BuddyFile>> Paginate(int path = 1, int size = 100);
}

public class FileDbService : OrmMapExtended<BuddyFile>, IFileDbService
{
	public FileDbService(IQueryService query, ISqlService sql) : base(query, sql) { }
}
