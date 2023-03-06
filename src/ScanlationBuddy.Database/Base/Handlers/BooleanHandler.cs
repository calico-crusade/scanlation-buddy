using Dapper;
using System.Data;

namespace ScanlationBuddy.Database.Base;

public class BooleanHandler : SqlMapper.TypeHandler<bool>
{
	public override bool Parse(object value)
	{
		return value switch
		{
			string str => bool.TryParse(str, out var res) ? res : false,
			int val => val == 1,
			long val => val == 1,
			float val => val == 1,
			_ => false
		};
	}

	public override void SetValue(IDbDataParameter parameter, bool value)
	{
		parameter.Value = value ? 1 : 0;
	}
}

public class NullableBooleanHandler : SqlMapper.TypeHandler<bool?>
{
	public override bool? Parse(object value)
	{
		if (value == null) return null;

		return value switch
		{
			string str => bool.TryParse(str, out var res) ? res : null,
			int val => val == 1,
			long val => val == 1,
			float val => val == 1,
			_ => null
		};
	}

	public override void SetValue(IDbDataParameter parameter, bool? value)
	{
		parameter.Value = value == null ? null : (value.Value ? 1 : 0);
	}
}
