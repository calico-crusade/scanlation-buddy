using Dapper;
using System.Data;

namespace ScanlationBuddy.Database.Base;

public class ArrayHandler<T> : SqlMapper.TypeHandler<T[]>
{
	public override T[] Parse(object value)
	{
		if (value == null || value is not string str) return Array.Empty<T>();

		return JsonSerializer.Deserialize<T[]>(str) ?? Array.Empty<T>();
	}

	public override void SetValue(IDbDataParameter parameter, T[] value)
	{
		value ??= Array.Empty<T>();
		parameter.Value = JsonSerializer.Serialize(value);
	}
}
