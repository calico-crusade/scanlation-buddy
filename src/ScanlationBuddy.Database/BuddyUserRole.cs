namespace ScanlationBuddy.Database;

public class BuddyUserRole : DbObject
{
	[JsonPropertyName("userId")]
	public long UserId { get; set; }

	[JsonPropertyName("roleId")]
	public long RoleId { get; set; }

	[JsonPropertyName("creatorId")]
	public long CreatorId { get; set; }
}
