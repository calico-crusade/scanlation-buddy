namespace ScanlationBuddy.Database;

public class BuddyAsset : DbObject
{
	[JsonPropertyName("fileId")]
	public long FileId { get; set; }

	[JsonPropertyName("replacedBy")]
	public long? ReplacedBy { get; set; }

	[JsonPropertyName("replacedWith")]
	public long? ReplacedWith { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("description")]
	public string Description { get; set; } = string.Empty;

	[JsonPropertyName("creatorId")]
	public long CreatorId { get; set; }
}
