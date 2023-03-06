namespace ScanlationBuddy;

public static class Permissions
{
	/// <summary>
	/// The person has permission to access the site
	/// </summary>
	public const string PERM_ACCESS_SITE = "Access Site";

	/// <summary>
	/// The person can change the roles of users
	/// </summary>
	public const string PERM_ADMIN_ROLES = "Admin - Grant Roles";

	/// <summary>
	/// The person can edit roles within the application
	/// </summary>
	public const string PERM_ADMIN_EDIT_ROLES = "Admin - Edit Roles";

	/// <summary>
	/// The person can create / import projects on the site 
	/// </summary>
	public const string PERM_PROJECT_CREATE = "Project - Create";


	public static Permission[] PermissionDescriptions = new Permission[]
	{
		new(PERM_ACCESS_SITE, "Permission to access the site"),
		
		new(PERM_ADMIN_ROLES, "Change / assign the roles of users."),
		new(PERM_ADMIN_EDIT_ROLES, "Create / edit roles within the application."),
		new(PERM_PROJECT_CREATE, "Create / import projects on the site.")
	};

	/// <summary>
	/// All of the perms in one array
	/// </summary>
	public static string[] PERMS_ALL => PermissionDescriptions.Select(t => t.Name).ToArray();
}

public class Permission
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("description")]
	public string Description { get; set; } = string.Empty;

	public Permission() { }

	public Permission(string name, string description)
	{
		Name = name;
		Description = description;
	}
}
