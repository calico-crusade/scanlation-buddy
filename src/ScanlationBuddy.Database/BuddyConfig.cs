namespace ScanlationBuddy.Database;

public class BuddyConfig : DbObject
{
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;

	[JsonPropertyName("value")]
	public string Value { get; set; } = string.Empty;

	[JsonPropertyName("description")]
	public string Description { get; set; } = string.Empty;

	[JsonPropertyName("groupName")]
	public string GroupName { get; set; } = string.Empty;

	public BuddyConfig() { }

	public BuddyConfig(string key, string value, string description, string groupName)
	{
		Key = key;
		Value = value;
		Description = description;
		GroupName = groupName;
	}

	public static BuddyConfig[] DEFAULTS = new BuddyConfig[]
	{
		new(CONFIG_DEFAULT_ROLES, "", "The default role(s) IDs to give users who sign up. If empty, it won't assign any role. " +
			"You can get the role IDs from the [User - Roles page](/users/roles), it's the number next to the role name. " +
			"If you want to give mutliple roles you can specify multiple separating them with commas.", "General"),
		new(CONFIG_WORKING_DIR, "sb-files", "The directory to put all files in. If you change this, you will need to manually move existing files to the new directory. " +
			"This can be an absolute path to a directory on your local machine, or a relative path from the applications working directory.", "General"),
		new(CONFIG_DISCORD_TOKEN, "", "The discord bot token. This is used for posting notifications for when manga chapters are published. " +
			"You can get this from the [discord developer panel](https://discord.com/developers), reach out on discord if you need help with this.", "Discord"),
		new(CONFIG_DISCORD_NOTI_CHANNEL, "", "The ID of the channel you want the discord bot to post publish notifications to.", "Discord"),
		new(CONFIG_DISCORD_NOTI_MESSAGE, "", "The default message you want the discord bot to send when posting publish notifications.", "Discord"),
	};
}
