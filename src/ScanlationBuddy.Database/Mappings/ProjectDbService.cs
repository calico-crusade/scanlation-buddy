namespace ScanlationBuddy.Database.Mappings
{

	public interface IProjectDbService
	{
		Task<(long id, bool isNew)> Upsert(BuddyProject project);

		Task<PaginatedResult<BuddyProject>> Paginate(int page = 1, int size = 100);

		Task<BuddyProject?> ByMdId(string id);
	}

	public class ProjectDbService : OrmMapExtended<BuddyProject>, IProjectDbService
	{
		public ProjectDbService(IQueryService query, ISqlService sql) : base(query, sql) { }

		public Task<(long id, bool isNew)> Upsert(BuddyProject project)
		{
			return Upsert(project, v => v.With(t => t.Hash), 
				updates: t => t.With(c => c.Id).With(c => c.CreatedAt).With(t => t.MangaDexId));
		}

		public Task<BuddyProject?> ByMdId(string id)
		{
			const string QUERY = "SELECT * FROM buddy_project WHERE manga_dex_id = :id AND deleted_at IS NULL";
			return _sql.Fetch<BuddyProject?>(QUERY, new { id });
		}
	}
}
