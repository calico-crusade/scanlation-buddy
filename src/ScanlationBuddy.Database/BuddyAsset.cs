namespace ScanlationBuddy.Database;

[Table("buddy_asset")]
public class BuddyAsset : UniqueDeletedDbObject
{
	[JsonPropertyName("fileId")]
	public long FileId { get; set; }

	[JsonPropertyName("replacedBy")]
	public long? ReplacedBy { get; set; }

	[JsonPropertyName("replacedWith")]
	public long? ReplacedWith { get; set; }

	[JsonPropertyName("name"), Column(Unique = true)]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("description")]
	public string Description { get; set; } = string.Empty;

	[JsonPropertyName("creatorId")]
	public long CreatorId { get; set; }
}
