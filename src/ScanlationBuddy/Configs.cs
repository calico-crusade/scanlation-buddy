namespace ScanlationBuddy;

public static class Configs
{
	/// <summary>
	/// The root directory to use for any files
	/// </summary>
	public const string CONFIG_WORKING_DIR = "Working Directory";
	
	/// <summary>
	/// The default role to give to users who create new accounts
	/// </summary>
	public const string CONFIG_DEFAULT_ROLES = "Default User Roles";

	/// <summary>
	/// The discord bots token
	/// </summary>
	public const string CONFIG_DISCORD_TOKEN = "Discord Bot Token";

	/// <summary>
	/// The ID of the channel to post publish notifications in.
	/// </summary>
	public const string CONFIG_DISCORD_NOTI_CHANNEL = "Discord Notification Channel Id";

	/// <summary>
	/// The default message to send as part of a publish notification.
	/// </summary>
	public const string CONFIG_DISCORD_NOTI_MESSAGE = "Discord Notification Message";
}
