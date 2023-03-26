namespace ScanlationBuddy.Database;

[Table("buddy_user_role")]
public class BuddyUserRole : DbObject
{
	[JsonPropertyName("userId"), Column(Unique = true)]
	public long UserId { get; set; }

	[JsonPropertyName("roleId"), Column(Unique = true)]
	public long RoleId { get; set; }

	[JsonPropertyName("creatorId")]
	public long CreatorId { get; set; }
}
