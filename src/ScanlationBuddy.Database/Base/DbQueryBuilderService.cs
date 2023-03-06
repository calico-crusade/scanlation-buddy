namespace ScanlationBuddy.Database;

public interface IDbQueryBuilderService
{
	string Insert<T>(string tableName, Action<PropertyExpressionBuilder<T>>? exclusions = null);
	string Insert(string tableName, Type type, params string[] exclusions);

	string InsertReturn<T, TReturn>(string tableName, Expression<Func<T, TReturn>> returnCol, Action<PropertyExpressionBuilder<T>>? exclusions = null);
	string InsertReturn(string tableName, Type type, string returnCol, params string[] exclusions);

	string Update<T>(string tableName, Action<PropertyExpressionBuilder<T>>? exclusions = null) where T : DbObject;
	string Update<T, TId>(string tableName, Expression<Func<T, TId>> id, Action<PropertyExpressionBuilder<T>>? exclusions = null);
	string Update(string tableName, Type type, PropertyMap id, params string[] exclusions);

	string Upsert<T>(string tableName,
		Action<PropertyExpressionBuilder<T>> conflicts,
		Action<PropertyExpressionBuilder<T>>? insertExclusions = null,
		Action<PropertyExpressionBuilder<T>>? updateExclusions = null,
		string? returning = null);
	string Upsert<T, TReturn>(string tableName,
		Action<PropertyExpressionBuilder<T>> conflicts,
		Action<PropertyExpressionBuilder<T>>? insertExclusions = null,
		Action<PropertyExpressionBuilder<T>>? updateExclusions = null,
		Expression<Func<T, TReturn>>? returnCol = null);
	string Upsert(string tableName, Type type, string[] conflicts, string[] insertExclusions, string[] updateExclusions, string? returning = null);

	string SelectAll(string tableName);
	string SelectAllNonDeleted(string tableName);
	string SelectId<T>(string tableName);
	string Select<T>(string tableName, Action<PropertyExpressionBuilder<T>> columns);
	string Select(string tableName, params PropertyMap[] columns);

	string Pagniate<T, TSort>(string tableName,
		Action<PropertyExpressionBuilder<T>> columns,
		Expression<Func<T, TSort>> order,
		bool asc = true,
		string sizeName = "size",
		string offsetName = "offset");
}

public class DbQueryBuilderService : IDbQueryBuilderService
{
	#region Helper Methods
	public virtual Dictionary<string, string> PropertyMask => new()
	{
		["updated_at"] = "CURRENT_TIMESTAMP",
		["created_at"] = "CURRENT_TIMESTAMP"
	};

	public PropertyName GetPropertyName(PropertyInfo prop) => new(prop.Name, ToSnakeCase(prop.Name));

	public PropertyName[] GetPropertyNames<T>(Action<PropertyExpressionBuilder<T>> action)
	{
		var bob = new PropertyExpressionBuilder<T>();
		action?.Invoke(bob);
		return bob.Properties.Select(GetPropertyName).ToArray();
	}

	public string[] GetPropertySnakes<T>(Action<PropertyExpressionBuilder<T>>? action)
	{
		var bob = new PropertyExpressionBuilder<T>();
		action?.Invoke(bob);
		return bob.Properties.Select(t => GetPropertyName(t).Snake).ToArray();
	}

	public PropertyName[] GetPropertyNames(Type type)
	{
		return type.GetProperties()
			.Select(GetPropertyName)
			.ToArray();
	}

	public PropertyMap[] GetPropertyMap(IEnumerable<PropertyName> names, string[] exclusions, bool applyTransforms = true, bool pascalCase = false)
	{
		return names
			.Where(t => !exclusions.Any(a => a == t.Snake))
			.Select(t =>
			{
				if (applyTransforms && PropertyMask.ContainsKey(t.Snake))
					return new PropertyMap(t.Snake, PropertyMask[t.Snake]);

				var orig = pascalCase ? ToPascalCase(t.Original) : t.Original;

				return new PropertyMap(t.Snake, ":" + orig);
			})
			.ToArray();
	}

	public bool AssignableFrom<T>(Type type)
	{
		return typeof(T).IsAssignableFrom(type);
	}
	#endregion

	#region Inserts
	public string[] DeteremineInsertExclusions(Type type)
	{
		if (AssignableFrom<DbObject>(type))
			return new[] { "id", "deleted_at" };

		return Array.Empty<string>();
	}

	public string Insert<T>(string tableName, Action<PropertyExpressionBuilder<T>>? exclusions = null)
	{
		var exlus = GetPropertySnakes(exclusions);
		return Insert(tableName, typeof(T), exlus);
	}

	public string Insert(string tableName, Type type, params string[] exclusions)
	{
		if (exclusions == null || exclusions.Length == 0)
			exclusions = DeteremineInsertExclusions(type);

		var props = GetPropertyMap(GetPropertyNames(type), exclusions);

		var columns = props.Select(t => t.Snake).ToArray();
		var values = props.Select(t => t.Value).ToArray();

		return $@"INSERT INTO {tableName} (
	{string.Join(",\r\n\t", columns)}
) VALUES (
	{string.Join(",\r\n\t", values)}
)";
	}

	public string InsertReturn<T, TReturn>(string tableName, Expression<Func<T, TReturn>> returnCol, Action<PropertyExpressionBuilder<T>>? exclusions = null)
	{
		var rc = GetPropertyName(GetPropertyInfo(returnCol)).Snake;
		var exlus = GetPropertySnakes(exclusions);
		return InsertReturn(tableName, typeof(T), rc, exlus);
	}

	public string InsertReturn(string tableName, Type type, string returnCol, params string[] exclusions)
	{
		return Insert(tableName, type, exclusions) + " RETURNING " + returnCol;
	}
	#endregion

	#region Updates
	public string[] DeteremineUpdateExclusions(Type type)
	{
		if (AssignableFrom<DbObject>(type))
			return new[] { "id", "created_at" };

		return Array.Empty<string>();
	}

	public string Update<T>(string tableName, Action<PropertyExpressionBuilder<T>>? exclusions = null) where T : DbObject
	{
		return Update(tableName, t => t.Id, exclusions);
	}

	public string Update<T, TId>(string tableName, Expression<Func<T, TId>> id, Action<PropertyExpressionBuilder<T>>? exclusions = null)
	{
		var idProp = GetPropertyInfo(id);
		var idCol = GetPropertyMap(new[] { GetPropertyName(idProp) }, Array.Empty<string>()).First();

		var exlus = GetPropertySnakes(exclusions);
		return Update(tableName, typeof(T), idCol, exlus);
	}

	public string Update(string tableName, Type type, PropertyMap id, params string[] exclusions)
	{
		if (exclusions == null || exclusions.Length == 0)
			exclusions = DeteremineUpdateExclusions(type);

		var props = GetPropertyMap(GetPropertyNames(type), exclusions);

		var columns = props.Select(t => $"{t.Snake} = {t.Value}");

		return $@"UPDATE {tableName}
SET
	{string.Join(",\r\n", columns)}
WHERE
	{id.Snake} = {id.Value}";
	}
	#endregion

	#region Upserts
	public string Upsert<T, TReturn>(string tableName,
		Action<PropertyExpressionBuilder<T>> conflicts,
		Action<PropertyExpressionBuilder<T>>? insertExclusions = null,
		Action<PropertyExpressionBuilder<T>>? updateExclusions = null,
		Expression<Func<T, TReturn>>? returnCol = null)
	{
		var rc = GetPropertyName(GetPropertyInfo(returnCol)).Snake;
		return Upsert(tableName, conflicts, insertExclusions, updateExclusions, rc);
	}

	public string Upsert<T>(string tableName,
		Action<PropertyExpressionBuilder<T>> conflicts,
		Action<PropertyExpressionBuilder<T>>? insertExclusions = null,
		Action<PropertyExpressionBuilder<T>>? updateExclusions = null,
		string? returning = null)
	{
		var ins = GetPropertySnakes(insertExclusions);
		var ups = GetPropertySnakes(updateExclusions);
		var confs = GetPropertySnakes(conflicts);
		return Upsert(tableName, typeof(T), confs, ins, ups, returning);
	}

	public string Upsert(string tableName, Type type, string[] conflicts, string[] insertExclusions, string[] updateExclusions, string? returning = null)
	{
		var insert = Insert(tableName, type, insertExclusions);

		var props = GetPropertyMap(GetPropertyNames(type), updateExclusions);

		var columns = props.Select(t => $"{t.Snake} = {t.Value}");

		var ret = !string.IsNullOrEmpty(returning) ? "\r\nRETURNING " + returning : "";

		return $@"{insert} ON CONFLICT ({string.Join(", ", conflicts)}) DO UPDATE SET
	{string.Join(",\r\n\t", columns)}{ret}";
	}
	#endregion

	#region Selects
	public string SelectAllNonDeleted(string tableName)
	{
		return $"SELECT * FROM {tableName} WHERE deleted_at IS NULL";

	}

	public string SelectAll(string tableName)
	{
		return $"SELECT * FROM {tableName}";
	}

	public string SelectId<T>(string tableName)
	{
		var type = typeof(T);
		if (AssignableFrom<DbObject>(type))
			return Select(tableName, new PropertyMap[]
			{
				new ("id", ":Id"),
				new ("deleted_at", "IS NULL", false)
			});

		throw new ArgumentException($"{type.Name} is not supported.");
	}

	public virtual string Select<T>(string tableName, Action<PropertyExpressionBuilder<T>> columns)
	{
		var propNames = GetPropertyNames(columns);
		var map = GetPropertyMap(propNames, Array.Empty<string>(), false, true);
		return Select(tableName, map);
	}

	public string Select(string tableName, params PropertyMap[] columns)
	{
		var cols = columns.Select(t => t.applyEquals ? $"{t.Snake} = {t.Value}" : $"{t.Snake} {t.Value}");
		return $@"SELECT
	*
FROM {tableName}
WHERE
	{string.Join(" AND\r\n\t", cols)}";
	}
	#endregion

	public string Pagniate<T, TSort>(string tableName,
		Action<PropertyExpressionBuilder<T>> columns,
		Expression<Func<T, TSort>> order, bool asc = true,
		string sizeName = "size", string offsetName = "offset")
	{
		var propNames = GetPropertyNames(columns);
		var map = GetPropertyMap(propNames, Array.Empty<string>(), false, true);
		var cols = map.Select(t => t.applyEquals ? $"{t.Snake} = {t.Value}" : $"{t.Snake} {t.Value}");
		var where = map.Length == 0 ? "" : "WHERE " + string.Join(" AND\r\n\t", cols);
		var sortProp = GetPropertyInfo(order);
		var sortCol = GetPropertyMap(new[] { GetPropertyName(sortProp) }, Array.Empty<string>()).First();

		return $@"SELECT
	*
FROM {tableName}
{where}
ORDER BY {sortCol.Snake} {(asc ? "ASC" : "DESC")}
LIMIT :{sizeName}
OFFSET :{offsetName};

SELECT COUNT(*) FROM {tableName} {where};";

	}

	public static string ToSnakeCase(string text)
	{
		if (text == null) throw new ArgumentNullException(nameof(text));
		if (text.Length < 2) return text;

		var sb = new StringBuilder();
		sb.Append(char.ToLowerInvariant(text[0]));
		for (int i = 1; i < text.Length; ++i)
		{
			char c = text[i];
			if (!char.IsUpper(c))
			{
				sb.Append(c);
				continue;
			}

			sb.Append('_');
			sb.Append(char.ToLowerInvariant(c));
		}

		return sb.ToString();
	}

	public static string ToPascalCase(string? text)
	{
		if (text == null) throw new ArgumentNullException(nameof(text));

		var chars = text.ToCharArray();
		chars[0] = char.ToLowerInvariant(chars[0]);
		return new string(chars);
	}

	public static PropertyInfo GetPropertyInfo<TSource, TProp>(Expression<Func<TSource, TProp>>? propertyLambda)
	{
		if (propertyLambda == null) throw new ArgumentNullException(nameof(propertyLambda));

		var type = typeof(TSource);

		if (propertyLambda.Body is not MemberExpression member)
			throw new ArgumentException(string.Format(
				"Expression '{0}' refers to a method, not a property.",
				propertyLambda.ToString()));

		var propInfo = member.Member as PropertyInfo;
		if (propInfo == null)
			throw new ArgumentException(string.Format(
				"Expression '{0}' refers to a field, not a property.",
				propertyLambda.ToString()));

		if (propInfo.ReflectedType != null &&
			type != propInfo.ReflectedType &&
			!type.IsSubclassOf(propInfo.ReflectedType))
			throw new ArgumentException(string.Format(
				"Expression '{0}' refers to a property that is not from type {1}.",
				propertyLambda.ToString(),
				type));

		return propInfo;
	}
}

public class SqliteDbQueryBuilderService : DbQueryBuilderService
{
	public override Dictionary<string, string> PropertyMask => new()
	{
		["updated_at"] = "datetime('now')",
		["created_at"] = "datetime('now')"
	};

	public override string Select<T>(string tableName, Action<PropertyExpressionBuilder<T>> columns)
	{
		var propNames = GetPropertyNames(columns);
		var map = GetPropertyMap(propNames, Array.Empty<string>(), false, false);
		return Select(tableName, map);
	}
}

public record class PropertyName(string Original, string Snake);
public record class PropertyMap(string Snake, string Value, bool applyEquals = true);
