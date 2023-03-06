﻿namespace ScanlationBuddy.Database;

public class BuddyUser : DbObject
{
	[JsonPropertyName("username")]
	public string Username { get; set; } = string.Empty;

	[JsonPropertyName("avatar")]
	public string Avatar { get; set; } = string.Empty;

	[JsonPropertyName("platformId")]
	public string PlatformId { get; set; } = string.Empty;

	[JsonPropertyName("roles")]
	public string Roles { get; set; } = string.Empty;

	[JsonIgnore]
	public string Email { get; set; } = string.Empty;

	[JsonPropertyName("provider")]
	public string Provider { get; set; } = string.Empty;

	[JsonPropertyName("providerId")]
	public string ProviderId { get; set; } = string.Empty;
}