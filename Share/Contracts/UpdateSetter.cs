

using System.Linq.Expressions;
using System.Reflection;



namespace Share.Contracts;

public class UpdateSetter {
    public string? MemberName { get; set; }

    public LambdaExpression? MemberExpression { get; set; }

    public LambdaExpression? ValueExpression { get; set; }

    public object? Value { get; set; }

    public Type ValueType { get; set; } = null!;

    public void Apply(object row)
    {
        var newValue = ValueExpression != null ? ValueExpression.Compile().DynamicInvoke(row) : Value;

        if (MemberExpression != null)
        {
            var memberInfo = ((MemberExpression)MemberExpression.Body).Member;

            if (memberInfo.MemberType != MemberTypes.Property)
                throw new NotSupportedException($"memberInfo.MemberType({memberInfo.MemberType}) is not supported");

            ((PropertyInfo)memberInfo).SetValue(row, newValue);
        }
        else if (MemberName != null)
        {
            var rowType = row.GetType();
            var property = rowType.GetProperty(MemberName);

            if (property == null)
                throw new Exception($"the property '{MemberName}' was not found in the type '{rowType.Name}'");

            property.SetValue(row, newValue);
        }
        else
        {
            throw new Exception("both MemberName and MemberExpression is null");
        }
    }
}