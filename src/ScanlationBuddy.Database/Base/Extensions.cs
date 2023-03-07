namespace ScanlationBuddy.Database;

using Base;
using Mappings;

public static class Extensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
	{
        MapConfig.AddMap(c =>
        {
            c.ForEntity<BuddyUser>()
             .ForEntity<BuddyProject>()
             .ForEntity<BuddyRole>()
             .ForEntity<BuddyRoleUser>()
             .ForEntity<BuddyUserRole>();
        });

        MapConfig.StartMap();

		static void SwitchMap<T, T2>() where T2: SqlMapper.TypeHandler<T>, new()
		{
            SqlMapper.RemoveTypeMap(typeof(T));
            SqlMapper.AddTypeHandler(new T2());
		}

        SwitchMap<DateTime, DateTimeHandler>();
        SwitchMap<DateTime?, NullableDateTimeHandler>();
        SwitchMap<bool, BooleanHandler>();
        SwitchMap<bool?, NullableBooleanHandler>();
        SwitchMap<string[], ArrayHandler<string>>();
		SwitchMap<int[], ArrayHandler<int>>();
        SwitchMap<double[], ArrayHandler<double>>();

        return services
			.AddSingleton<ISqlService, SqliteService>()
            .AddTransient<IDbQueryBuilderService, SqliteDbQueryBuilderService>()
            
            .AddTransient<IUserDbService, UserDbService>()
            .AddTransient<IProjectDbService, ProjectDbService>()
            .AddTransient<IRoleDbService, RoleDbService>()

            .AddTransient<IDbService, DbService>();
	}
}