namespace ScanlationBuddy.Database;

using Mappings;

public static class Extensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
	{
        return services
            .AddSqlService(c =>
            {
                c.AddSQLite<SqliteConfig>(f => f.OnInit(async con => 
                {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
                    if (!Directory.Exists(path)) return;

                    var files = Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories)
                        .OrderBy(t => Path.GetFileName(t))
                        .ToArray();

                    if (files.Length <= 0) return;

                    foreach(var file in files)
                    {
                        var context = await File.ReadAllTextAsync(file);
                        await con.ExecuteAsync(context);
                    }
                }))
                .ConfigureGeneration(c => c.WithCamelCaseChange())
                .ConfigureTypes(c => 
                {
                    c.CamelCase()
                     .Entity<BuddyUser>()
                     .Entity<BuddyProject>()
                     .Entity<BuddyRole>()
                     .Entity<BuddyRoleUser>()
                     .Entity<BuddyUserRole>()
                     .Entity<BuddyFile>()
                     .Entity<BuddyConfig>()
                     .Entity<BuddyAsset>();

                    c.PolyfillBooleanHandler()
                     .PolyfillDateTimeHandler()
                     .ArrayHandler<string>()
                     .ArrayHandler<int>()
                     .ArrayHandler<double>();
                });
            })            
            .AddTransient<IUserDbService, UserDbService>()
            .AddTransient<IProjectDbService, ProjectDbService>()
            .AddTransient<IRoleDbService, RoleDbService>()
            .AddTransient<IFileDbService, FileDbService>()
            .AddTransient<IConfigDbService, ConfigDbService>()

            .AddTransient<IDbService, DbService>();
	}
}