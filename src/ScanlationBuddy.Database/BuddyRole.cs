﻿namespace ScanlationBuddy.Database;

[Table("buddy_role")]
public class BuddyRole : UniqueDeletedDbObject
{
	[JsonPropertyName("name"), Column(Unique = true)]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("permissions")]
	public string[] Permissions { get; set; } = Array.Empty<string>();

	[JsonPropertyName("creatorId")]
	public long CreatorId { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; set; } = string.Empty;

	[JsonPropertyName("badgeId")]
	public string? BadgeId { get; set; }

	[JsonPropertyName("color")]
	public string Color { get; set; } = "#fff";
}

public class BuddyRoleUser : BuddyRole
{
	[JsonPropertyName("userId")]
	public long UserId { get; set; }
}
