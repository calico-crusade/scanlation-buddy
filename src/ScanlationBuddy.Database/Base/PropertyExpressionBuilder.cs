namespace ScanlationBuddy.Database;

public interface IPropertyExpressionBuilder<T>
{
	IPropertyExpressionBuilder<T> With<TProp>(Expression<Func<T, TProp>> property);
}

public class PropertyExpressionBuilder<T> : IPropertyExpressionBuilder<T>
{
	public List<PropertyInfo> Properties { get; } = new();

	public IPropertyExpressionBuilder<T> With<TProp>(Expression<Func<T, TProp>> property)
	{
		Properties.Add(DbQueryBuilderService.GetPropertyInfo(property));
		return this;
	}
}
