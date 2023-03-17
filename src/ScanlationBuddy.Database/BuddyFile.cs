namespace ScanlationBuddy.Database;

public class BuddyFile : DbObject
{
	[JsonPropertyName("filename")]
	public string Filename { get; set; } = string.Empty;

	[JsonPropertyName("hash")]
	public string Hash { get; set; } = string.Empty;

	[JsonPropertyName("mimeType")]
	public string MimeType { get; set; } = string.Empty;

	[JsonPropertyName("length")]
	public long Length { get; set; }

	[JsonPropertyName("creatorId")]
	public long CreatorId { get; set; }
}
