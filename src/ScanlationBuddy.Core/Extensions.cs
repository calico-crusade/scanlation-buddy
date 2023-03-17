namespace ScanlationBuddy.Core;

public static class Extensions
{
	public static IServiceCollection AddCore(this IServiceCollection services)
	{
		return services
			.AddTransient<IUserService, UserService>()
			.AddTransient<IFileUploadService, FileUploadService>();
	}
}
