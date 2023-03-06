using Dapper;

namespace ScanlationBuddy.Database;

using Base;
using Mappings;

public static class Extensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
	{
        MapConfig.AddMap(c =>
        {
            c.ForEntity<BuddyUser>();
        });

        MapConfig.StartMap();

        SqlMapper.RemoveTypeMap(typeof(DateTime));
        SqlMapper.RemoveTypeMap(typeof(DateTime?));
        SqlMapper.RemoveTypeMap(typeof(bool));
        SqlMapper.RemoveTypeMap(typeof(bool?));
        SqlMapper.AddTypeHandler(new DateTimeHandler());
        SqlMapper.AddTypeHandler(new NullableDateTimeHandler());
        SqlMapper.AddTypeHandler(new BooleanHandler());
        SqlMapper.AddTypeHandler(new NullableBooleanHandler());

        return services
			.AddSingleton<ISqlService, SqliteService>()
            .AddTransient<IDbQueryBuilderService, SqliteDbQueryBuilderService>()
            
            .AddTransient<IUserDbService, UserDbService>()

            .AddTransient<IDbService, DbService>();
	}
}