namespace ScanlationBuddy.Database;

[Table("buddy_project")]
public class BuddyProject : DbObject
{
	[JsonPropertyName("hash")]
	public string Hash { get; set; } = string.Empty;

	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("notes")]
	public string Notes { get; set; } = string.Empty;

	[JsonPropertyName("mangaDexId")]
	public string? MangaDexId { get; set; }

	[JsonPropertyName("state")]
	public ProjectState State { get; set; }

	[JsonPropertyName("creatorId")]
	public long CreatorId { get; set; }
}

public enum ProjectState
{
	None = 0,
	OnGoing = 1,
	Hiatus = 2,
	Completed = 3,
	Abandoned = 4,
	Potential = 5,
}