#nullable enable

namespace CompiledBindings;

public static class BindingParser
{
	public static Bind Parse(XamlObjectProperty prop, TypeInfo sourceType, TypeInfo targetType, string dataRootName, BindingMode defaultBindMode, XamlDomParser xamlDomParser, ref int localVarIndex)
	{
		var xBind = prop.XamlNode.Children[0];
		var str = xBind.Value?.TrimEnd();
		if (string.IsNullOrWhiteSpace(str))
		{
			throw new ParseException("Missing expression.");
		}

		var namespaces = xamlDomParser.GetNamespaces(prop.XamlNode).ToList();

		Expression? expression = null;
		Expression? bindBackExpression = null;
		Expression? converter = null;
		Expression? converterParameter = null;
		Expression? fallbackValue = null;
		Expression? targetNullValue = null;
		TypeInfo? dataType = null;
		bool dataTypeSet = false;
		BindingMode? mode = null;
		bool isItemsSource = false;
		UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Event;
		EventInfo? targetChangedEvent = null;
		List<(string name, TypeInfo type)> resources = new();

		// TODO! how to match optional comma and not include it in the group?
		var match = Regex.Match(str, @"^.*DataType\s*(?<!=)=(?!=)(.+)");
		if (match.Success)
		{
			var typeExpr = match.Groups[1].Value.Trim();
			var pos = typeExpr.IndexOf(',');
			if (pos != -1)
			{
				typeExpr = typeExpr.Substring(0, pos);
			}
			var type = xamlDomParser.FindType(typeExpr, (XAttribute)prop.XamlNode.Element);
			dataType = type == null ? null : new TypeInfo(type, false);
			dataTypeSet = true;

			sourceType = dataType ?? targetType;
		}

		int currentPos = 0, pos1 = 0;
		while (true)
		{
			if (pos1 == str!.Length)
			{
				break;
			}

			var str2 = str.Substring(pos1).TrimStart();
			if (str2.Length == 0)
			{
				break;
			}

			currentPos += str.Length - str2.Length;
			str = str2;

			bool parseExpression = false;
			match = Regex.Match(str, @"^(\w+)\s*=\s*(.+)\s*$");
			if (!match.Success)
			{
				if (expression == null)
				{
					parseExpression = true;
				}
				else
				{
					throw new ParseException($"Syntax error.");
				}
			}

			string name;
			if (parseExpression)
			{
				if (expression != null)
				{
					throw new ParseException($"Syntax error.");
				}
				name = "Path";
			}
			else
			{
				name = match.Groups[1].Value;
				str = match.Groups[2].Value;
			}

			if (name is "Path" or "BindBack" or "Converter" or "ConverterParameter" or "FallbackValue" or "TargetNullValue")
			{
				Expression expr;
				if (name is "Converter" or "ConverterParameter" or "FallbackValue" or "TargetNullValue" &&
					Regex.Match(str, @"{StaticResource\s+(\w+)}") is var match2 && match2.Success)
				{
					var resourceName = match2.Groups[1].Value;
					var resourceType = name switch
					{
						"Converter" => xamlDomParser.ConverterType,
						"FallbackValue" or "TargetNullValue" => prop.MemberType,
						_ => TypeInfo.GetTypeThrow(typeof(object))
					};
					resources.Add((resourceName, resourceType));

					var resourceField = new FieldInfo(new FieldDefinition(resourceName, FieldAttributes.Private, xamlDomParser.ConverterType.Type), xamlDomParser.ConverterType);
					expr = new ParameterExpression(targetType, "targetRoot");
					expr = new MemberExpression(expr, resourceField, new TypeInfo(xamlDomParser.ConverterType, false));

					int pos2 = str.IndexOf(',');
					if (pos2 == -1)
					{
						pos1 = pos2 = str.Length;
					}
					else
					{
						pos1 = pos2 + 1;
					}
				}
				else
				{
					IList<XamlNamespace> includeNamespaces;
					try
					{
						expr = ExpressionParser.Parse(sourceType, dataRootName, str, prop.MemberType, false, namespaces, out includeNamespaces, out pos1);
					}
					catch (ParseException ex)
					{
						throw new ParseException(ex.Message, currentPos + ex.Position, ex.Length);
					}
					includeNamespaces.ForEach(ns => prop.IncludeNamespaces.Add(ns.ClrNamespace!));
				}
				if (name == "Path")
				{
					expression = expr;
				}
				else if (name == "BindBack")
				{
					bindBackExpression = expr;
					if (mode == null)
					{
						mode = BindingMode.TwoWay;
					}
				}
				else if (name == "FallbackValue")
				{
					fallbackValue = expr;
				}
				else if (name == "TargetNullValue")
				{
					targetNullValue = expr;
				}
				else if (name == "Converter")
				{
					converter = expr;
				}
				else
				{
					converterParameter = expr;
				}
			}
			else if (name is "Mode" or "UpdateSourceTrigger" or "DataType" or "IsItemsSource")
			{
				int pos2 = str.IndexOf(',');
				if (pos2 == -1)
				{
					pos1 = pos2 = str.Length;
				}
				else
				{
					pos1 = pos2 + 1;
				}

				var value = str.Substring(0, pos2).Trim();

				if (name == "Mode")
				{
					if (Enum.TryParse<BindingMode>(value, out var mode2))
					{
						mode = mode2;
					}
					else
					{
						var msg = $"Mode is invalid: {value}.";
						if (expression == null)
						{
							msg += " Use 'eq' instead of '=' to compare 'Mode' in expression.";
						}
						throw new ParseException(msg, currentPos + match.Groups[2].Index);
					}
				}
				else if (name == "DataType")
				{
					// The DataType was processed above
				}
				else if (name == "IsItemsSource")
				{
					if (!bool.TryParse(value, out isItemsSource))
					{
						throw new ParseException($"Invalid boolean value: {value}", currentPos + match.Groups[2].Index);
					}
				}
				else
				{
					if (value == "Explicit")
					{
						updateSourceTrigger = UpdateSourceTrigger.Explicit;
					}
					else
					{
						targetChangedEvent = prop.Object.Type.Events.FirstOrDefault(e => e.Definition.Name == value);
						if (targetChangedEvent == null)
						{
							throw new ParseException($"The type {prop.Object.Type.Type.FullName} does not have event {value}.", currentPos + match.Groups[2].Index);
						}
					}
				}
			}
			else
			{
				throw new ParseException($"Property {name} is not valid for x:Bind", currentPos);
			}
		}

		if (expression == null)
		{
			if (mode != BindingMode.OneWayToSource)
			{
				throw new ParseException("Missing expression.");
			}
			if (bindBackExpression == null)
			{
				throw new ParseException("Missing Path or BindBack expression.");
			}
		}

		if (mode is BindingMode.TwoWay or BindingMode.OneWayToSource && expression is not (MemberExpression or CallExpression or ElementAccessExpression))
		{
			throw new ParseException("The expression must be settable for TwoWay or OneWayToSource bindings.");
		}

		if (isItemsSource && expression == null)
		{
			throw new ParseException("IsItemsSource cannot be used for OneWayToSource bindings.");
		}

		if ((mode is BindingMode.TwoWay or BindingMode.OneWayToSource) && targetChangedEvent == null && updateSourceTrigger != UpdateSourceTrigger.Explicit)
		{
			var iNotifyPropChanged = TypeInfo.GetTypeThrow(typeof(INotifyPropertyChanged));
			if (iNotifyPropChanged.IsAssignableFrom(prop.Object.Type))
			{
				targetChangedEvent = iNotifyPropChanged.Events.First();
			}
		}
		// Note! It is not checked now whether an event is set for a not explicit two way binding.
		// The platform generator can add default events for some controls.
		// Whether an event is set, is checked in code generator.

		var sourceExpression = expression;

		if (sourceExpression != null && converter != null)
		{
			var convertMethod = xamlDomParser.ConverterType.Methods.First(m => m.Definition.Name == "Convert");
			sourceExpression = new CallExpression(converter, convertMethod, new Expression[]
			{
					sourceExpression,
					new TypeofExpression(new TypeExpression(prop.MemberType)),
					converterParameter ?? Expression.NullExpression,
					Expression.NullExpression
			});
			if (prop.MemberType.Type.FullName != "System.Object")
			{
				sourceExpression = new CastExpression(sourceExpression, prop.MemberType, false);
			}
		}

		if (sourceExpression?.IsNullable == true && targetNullValue != null)
		{
			sourceExpression = new CoalesceExpression(sourceExpression, targetNullValue);
		}

		if (sourceExpression != null && fallbackValue != null)
		{
			var expr = sourceExpression.EnumerateTree().Reverse().OfType<IAccessExpression>().FirstOrDefault(e => e.Expression.IsNullable);
			if (expr != null)
			{
				var localVarName = "v" + localVarIndex++;
				sourceExpression = new FallbackExpression(expr.Expression, fallbackValue, expr.CloneReplaceExpression(new ParameterExpression(new TypeInfo(sourceExpression.Type, false), localVarName)), localVarName, sourceExpression.Type);
			}
		}

		return new Bind
		{
			Property = prop,
			DataType = dataType,
			DataTypeSet = dataTypeSet,
			Expression = expression,
			BindBackExpression = bindBackExpression,
			Converter = converter,
			ConverterParameter = converterParameter,
			FallbackValue = fallbackValue,
			Mode = mode ?? defaultBindMode,
			IsItemsSource = isItemsSource,
			UpdateSourceTrigger = updateSourceTrigger,
			TargetChangedEvent = targetChangedEvent,
			Resources = resources,
			SourceExpression = sourceExpression,
		};
	}

	public static BindingsData CreateBindingsData(IList<Bind> binds, TypeInfo? targetType, TypeInfo dataType, TypeInfo? dependencyObjectType = null)
	{
		for (int i = 0; i < binds.Count; i++)
		{
			binds[i].Index = i;
		}

		var iNotifyPropertyChangedType = TypeInfo.GetTypeThrow(typeof(INotifyPropertyChanged));

		var notifyPropertyChangedList = binds
			.Where(b => b.Property.TargetEvent == null &&
						b.SourceExpression != null &&
						b.Mode is not (BindingMode.OneTime or BindingMode.OneWayToSource))
			.SelectMany(b => b.SourceExpression!.EnumerateTree().OfType<MemberExpression>().Select(e => (bind: b, expr: e)))
			.Where(e => CheckPropertyNotifiable(e.expr))
			.GroupBy(e => e.expr.Expression.ToString())
			.Select(g => new NotifyPropertyChangedData
			{
				SourceExpression = g.First().expr.Expression,
				Properties = g.GroupBy(e => e.expr.Member.Definition.Name).Select(g2 =>
				{
					var expr = g2.First().expr;
					return new NotifyPropertyChangedProperty
					{
						Property = (PropertyInfo)expr.Member,
						Expression = expr,
						Bindings = g2.Select(e => e.bind).Distinct().ToList(),
					};
				})
				.ToList(),
			})
			.OrderBy(g => g.SourceExpression.ToString())
			.ToList();

		bool CheckPropertyNotifiable(MemberExpression expr)
		{
			return expr.Member is PropertyInfo pi &&
				   !pi.Definition.IsStatic() &&
				   (iNotifyPropertyChangedType.IsAssignableFrom(expr.Expression.Type) ||
				   (dependencyObjectType?.IsAssignableFrom(expr.Expression.Type) == true && expr.Expression.Type.Fields.Any(f => f.Definition.Name == pi.Definition.Name + "Property"))) &&
				   !pi.Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ComponentModel.ReadOnlyAttribute" && (bool)a.ConstructorArguments[0].Value == true);
		}

		for (int i = 0; i < notifyPropertyChangedList.Count; i++)
		{
			notifyPropertyChangedList[i].Index = i;
		}

		foreach (var notifPropData in notifyPropertyChangedList)
		{
			foreach (var prop in notifPropData.Properties)
			{
				var expr = prop.Expression.ToString();
				foreach (var notifPropData2 in notifyPropertyChangedList
					.Where(g => g != notifPropData && GetSourceExpr(g.SourceExpression).ToString().StartsWith(expr)))
				{
					var notifPropData2Clone = notifPropData2.Clone();
					prop.DependentNotifyProperties.Add(notifPropData2Clone);
				}

				static Expression GetSourceExpr(Expression expr)
				{
					if (expr is ParenExpression pe)
					{
						return GetSourceExpr(pe.Expression);
					}
					else if (expr is CastExpression ce)
					{
						return GetSourceExpr(ce.Expression);
					}
					return expr;
				}
			}
		}

		foreach (var propChangeData in notifyPropertyChangedList)
		{
			var typedSender = new ParameterExpression(new TypeInfo(propChangeData.SourceExpression.Type, false), "typedSender");
			foreach (var prop in propChangeData.Properties)
			{
				var expr = new MemberExpression(typedSender, prop.Property, prop.Property.PropertyType);
				var setExpressions = prop.Bindings
					.Select(b =>
					{
						var expression = b.SourceExpression!.CloneReplace(prop.Expression, expr);
						return new PropertySetExpression(b.Property, expression);
					})
					.ToList();

				foreach (var prop2 in prop.DependentNotifyProperties)
				{
					setExpressions.Add(new PropertySetExpression(prop2.Properties[0].Bindings[0].Property, prop2.SourceExpression.CloneReplace(prop.Expression, expr)));
				}

				var localVars = ExpressionUtils.GroupExpressions(setExpressions);

				for (int i1 = prop.Bindings.Count, i2 = 0; i2 < prop.DependentNotifyProperties.Count; i1++, i2++)
				{
					prop.DependentNotifyProperties[i2].SourceExpression = setExpressions[i1].Expression;
				}

				prop.UpdateMethod = new UpdateMethod
				{
					LocalVariables = localVars,
					SetExpressions = setExpressions.Take(prop.Bindings.Count).Cast<PropertySetExpression>().ToList()
				};
			}
		}

		var twoWayEventHandlers = binds
			.Where(b => (b.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource) && b.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
			.GroupBy(b => (b.Property.Object, b.TargetChangedEvent))
			.Select((g, i) => new TwoWayEventData
			{
				Bindings = g.ToList(),
				Index = i
			})
			.ToList();

		var props1 = binds
			.Where(b => b.Property.TargetEvent == null && b.Mode != BindingMode.OneWayToSource)
			.Select(b => new PropertySetExpression(b.Property, b.SourceExpression!)).ToList();

		var props2 = notifyPropertyChangedList
			.Select(g => new PropertySetExpression(g.Properties[0].Bindings[0].Property, g.SourceExpression))
			.ToList();

		var props3 = props1.Union(props2).ToList();

		var localVars2 = ExpressionUtils.GroupExpressions(props3);

		for (int i = 0; i < props2.Count; i++)
		{
			notifyPropertyChangedList[i].SourceExpression = props2[i].Expression;
		}

		var updateMethod = new UpdateMethod
		{
			LocalVariables = localVars2,
			SetExpressions = props1
		};

		return new BindingsData
		{
			DataType = dataType,
			TargetType = targetType,
			Bindings = binds,
			NotifyPropertyChangedList = notifyPropertyChangedList,
			TwoWayEvents = twoWayEventHandlers,
			UpdateMethod = updateMethod
		};
	}
}

public class BindingsData
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public TypeInfo? TargetType { get; init; }
	public TypeInfo DataType { get; init; }
	public IList<Bind> Bindings { get; init; }
	public List<NotifyPropertyChangedData> NotifyPropertyChangedList { get; init; }
	public List<TwoWayEventData> TwoWayEvents { get; init; }
	public UpdateMethod UpdateMethod { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
};

public class NotifyPropertyChangedData
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public Expression SourceExpression { get; set; }
	public List<NotifyPropertyChangedProperty> Properties { get; init; }
	public int Index { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public NotifyPropertyChangedData Clone()
	{
		return (NotifyPropertyChangedData)MemberwiseClone();
	}
};

public class NotifyPropertyChangedProperty
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public PropertyInfo Property { get; init; }
	public Expression Expression { get; init; }
	public List<Bind> Bindings { get; init; }
	public UpdateMethod UpdateMethod { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public List<NotifyPropertyChangedData> DependentNotifyProperties { get; } = new List<NotifyPropertyChangedData>();
};

public class TwoWayEventData
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public List<Bind> Bindings { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public int Index { get; init; }
};

public class Bind
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public XamlObjectProperty Property { get; init; }
	public Expression? Expression { get; init; }
	public Expression? BindBackExpression { get; init; }
	public BindingMode Mode { get; init; }
	public bool IsItemsSource { get; init; }
	public Expression? Converter { get; init; }
	public Expression? ConverterParameter { get; init; }
	public Expression? FallbackValue { get; init; }
	public UpdateSourceTrigger? UpdateSourceTrigger { get; init; }
	public TypeInfo? DataType { get; init; }
	public bool DataTypeSet { get; init; }
	public EventInfo? TargetChangedEvent { get; set; }
	public List<(string name, TypeInfo type)> Resources { get; init; }

	public Expression? SourceExpression { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public bool IsDPChangeEvent;
	public int Index;
}

public enum UpdateSourceTrigger
{
	Event,
	Explicit,
}

public enum BindingMode
{
	OneTime = 0x00,
	OneWay = 0x01,
	OneWayToSource = 0x10,
	TwoWay = 0x11,
}
