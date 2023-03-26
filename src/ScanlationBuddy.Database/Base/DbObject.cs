namespace ScanlationBuddy.Database;

public abstract class DbObject
{
	[JsonPropertyName("id")]
	[Column(PrimaryKey = true, ExcludeInserts = true, ExcludeUpdates = true)]
	public virtual long Id { get; set; }

	[JsonPropertyName("createdAt")]
	[Column(ExcludeInserts = true, ExcludeUpdates = true, OverrideValue = "CURRENT_TIMESTAMP")]
	public virtual DateTime? CreatedAt { get; set; }

	[JsonPropertyName("updatedAt")]
	[Column(ExcludeInserts = true, OverrideValue = "CURRENT_TIMESTAMP")]
	public virtual DateTime? UpdatedAt { get; set; }

	[JsonPropertyName("deletedAt")]
	public virtual DateTime? DeletedAt { get; set; }
}

public abstract class UniqueDeletedDbObject : DbObject
{
	[JsonPropertyName("deletedAt"), Column(Unique = true)]
	public override DateTime? DeletedAt { get; set; }
}
