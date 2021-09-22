using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledBindings
{
	public class ExpressionUtils
	{
	}

	public class PropertySetExpressionBase
	{
		public PropertySetExpressionBase(Expression expression)
		{
			Expression = expression;
		}

		public Expression Expression { get; set; }
	}

	public class PropertySetExpression : PropertySetExpressionBase
	{
		public PropertySetExpression(XamlObjectProperty property, Expression expression) : base(expression)
		{
			Property = property;
		}

		public XamlObjectProperty Property { get; }
	}

	public class LocalVariable
	{
		public LocalVariable(string name, Expression expression)
		{
			Name = name;
			Expression = expression;
		}

		public string Name { get; }
		public Expression Expression { get; }
	}

	public class UpdateMethod
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public List<LocalVariable> LocalVariables { get; init; }
		public IReadOnlyList<PropertySetExpression> SetExpressions { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public List<XamlObjectProperty>? SetProperties { get; set; }

		public bool IsEmpty => SetExpressions.Count == 0 && SetProperties?.Count is not > 0;
	}
}
