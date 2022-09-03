namespace CompiledBindings;

public static class BindingParser
{
	public static Bind Parse(XamlDomBase xamlDom, XamlObjectProperty prop, TypeInfo sourceType, TypeInfo targetType, string dataRootName, BindingMode defaultBindMode, XamlDomParser xamlDomParser, bool throwIfWithoutDataType, ref int localVarIndex)
	{
		var xBind = prop.XamlNode.Children[0];
		var str = xBind.Value?.TrimEnd();
		if (string.IsNullOrWhiteSpace(str))
		{
			throw new ParseException(Res.MissingExpression);
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
		EventInfo? targetChangedEvent = null;
		List<(string name, TypeInfo type)> resources = new();

		// Try to find DataType property in the binding before parsing any expression
		// TODO: how to match optional comma and not include it in the group?
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
		else if (throwIfWithoutDataType)
		{
			throw new ParseException(Res.NoDataType);
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

			string name;
			match = Regex.Match(str, @"^(\w+)\s*(?<!=)=(?!=)\s*(.+)\s*$");
			if (match.Success)
			{
				name = match.Groups[1].Value;

				if (name == "Path" && expression != null)
				{
					throw new ParseException(Res.SyntaxError);
				}

				str = match.Groups[2].Value;
			}
			else if (expression == null)
			{
				name = "Path";
			}
			else
			{
				throw new ParseException(Res.SyntaxError);
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

					var resourceField = new FieldInfo(new FieldDefinition(resourceName, FieldAttributes.Private, resourceType.Type), resourceType);
					expr = new VariableExpression(targetType, "_targetRoot");
					expr = new MemberExpression(expr, resourceField, new TypeInfo(resourceType, false));

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
					includeNamespaces.ForEach(ns => xamlDom.AddNamespace(ns.ClrNamespace!));
				}
				if (name == "Path")
				{
					expression = expr;
				}
				else if (name == "BindBack")
				{
					bindBackExpression = expr;
					mode ??= BindingMode.TwoWay;
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
					targetChangedEvent = prop.Object.Type.Events.FirstOrDefault(e => e.Definition.Name == value);
					if (targetChangedEvent == null)
					{
						throw new ParseException($"The type {prop.Object.Type.Type.FullName} does not have event {value}.", currentPos + match.Groups[2].Index);
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

		if (mode is BindingMode.TwoWay or BindingMode.OneWayToSource && (bindBackExpression ?? expression) is not (MemberExpression or CallExpression or ElementAccessExpression))
		{
			throw new ParseException("The expression must be settable for TwoWay or OneWayToSource bindings.");
		}

		if (isItemsSource && expression == null)
		{
			throw new ParseException("IsItemsSource cannot be used for OneWayToSource bindings.");
		}

		if ((mode is BindingMode.TwoWay or BindingMode.OneWayToSource) && targetChangedEvent == null)
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
			var expr = sourceExpression
				.EnumerateTree().OrderByDescending(e => e.CSharpCode).OfType<IAccessExpression>().FirstOrDefault(e => e.Expression.IsNullable);
			if (expr != null)
			{
				var localVarName = "v" + localVarIndex++;
				sourceExpression = new FallbackExpression(
					sourceExpression,
					fallbackValue,
					localVarName);
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
		var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));

		// Go through all expressions in bindings and find notifiable properties, grouped by notifiable source
		var notifySources = binds
			.Where(b => b.Property.TargetEvent == null &&
						b.SourceExpression != null &&
						b.Mode is not (BindingMode.OneTime or BindingMode.OneWayToSource))
			.SelectMany(b => b.SourceExpression!.EnumerateTree().OfType<MemberExpression>().Select(e => (bind: b, expr: e)))
			.Where(e => CheckPropertyNotifiable(e.expr))
			.GroupBy(e => e.expr.Expression.CSharpCode)
			.Select(g =>
			{
				var expr1 = g.First().expr.Expression;
				var d = new NotifySource
				{
					Expression = expr1,
					SourceExpression = expr1,
				};
				d.Properties = g.GroupBy(e => e.expr.Member.Definition.Name).Select(g2 =>
				{
					var expr2 = g2.First().expr;
					var bindings = g2.Select(e => e.bind).Distinct().ToList();
					return new NotifyProperty
					{
						Parent = d,
						Property = (PropertyInfo)expr2.Member,
						Expression = expr2,
						SourceExpression = expr2,
						Bindings = new ReadOnlyCollection<Bind>(bindings),
						SetBindings = bindings.ToList(), // Make copy
					};
				})
				.ToList();
				return d;
			})
			.OrderBy(g => g.Expression.CSharpCode)
			.ToList();

		bool CheckPropertyNotifiable(MemberExpression expr)
		{
			return expr.Member is PropertyInfo pi &&
				   !pi.Definition.IsStatic() &&
				   (iNotifyPropertyChangedType.IsAssignableFrom(expr.Expression.Type) ||
				   (dependencyObjectType?.IsAssignableFrom(expr.Expression.Type) == true && expr.Expression.Type.Fields.Any(f => f.Definition.Name == pi.Definition.Name + "Property"))) &&
				   !pi.IsReadOnly;
		}

		// Set indexes to notifiable source used as IDs
		for (int i = 0; i < notifySources.Count; i++)
		{
			notifySources[i].Index = i;
		}

		foreach (var notifySource in notifySources.OrderByDescending(d => GetSourceExpr(d.Expression).CSharpCode))
		{
			foreach (var prop in notifySource.Properties)
			{
				var expr = prop.Expression.CSharpCode;
				foreach (var notifPropData2 in notifySources
					.Where(g => g != notifySource && GetSourceExpr(g.Expression).EnumerateTree().Any(e => e.CSharpCode.Equals(expr))))
				{
					// Skip the notification source, if it's already added to some child
					if (!prop.DependentNotifySources
						.SelectTree(p => p.Properties.SelectMany(p2 => p2.DependentNotifySources))
						.Any(d => d.Index == notifPropData2.Index))
					{
						notifPropData2.Parent = prop;
						var notifPropData2Clone = notifPropData2.Clone();
						prop.DependentNotifySources.Add(notifPropData2Clone);
					}

					// The bindings are set in the "child" UpdateXX_XX method.
					foreach (var b in notifPropData2.Properties.SelectMany(p => p.Bindings))
					{
						prop.SetBindings.Remove(b);
					}
				}
			}
		}

		foreach (var notifySource in notifySources)
		{
			if (notifySource.Properties.Count > 1 &&
				iNotifyPropertyChangedType.IsAssignableFrom(notifySource.SourceExpression.Type))
			{
				var bindings = notifySource.Properties.SelectMany(_ => _.Bindings).Distinct().ToList();
				var value = new VariableExpression(notifySource.SourceExpression.Type, "value");
				notifySource.UpdateMethod = CreateUpdateMethodData(bindings, null, notifySource, e => e.CloneReplace(notifySource.SourceExpression, value));
			}

			foreach (var prop in notifySource.Properties)
			{
				var type = prop.Property.PropertyType;
				if (!type.IsNullable && prop.Expression.IsNullable)
				{
					type = new TypeInfo(type, true);
				}
				var expr = new VariableExpression(type, "value");

				prop.UpdateMethod = CreateUpdateMethodData(prop.SetBindings, prop.DependentNotifySources, null, e => e.CloneReplace(prop.Expression, expr));
			}
		}

		var binds1 = binds
			.Where(b => b.Property.TargetEvent == null && b.Mode != BindingMode.OneWayToSource)
			.ToList();
		var updateMethod = CreateUpdateMethodData(binds1, notifySources, null, e => e);

		var twoWayEventHandlers1 = binds.Where(b => b.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource);

		var twoWayEventHandlers2 = twoWayEventHandlers1
			.Where(b => b.DependencyProperty != null)
			.GroupBy(b => (b.Property.Object, b.DependencyProperty))
			.Select(g => new TwoWayEventData
			{
				Bindings = g.ToList(),
			});

		var twoWayEventHandlers3 = twoWayEventHandlers1
			.Where(b => b.DependencyProperty == null)
			.GroupBy(b => (b.Property.Object, b.TargetChangedEvent))
			.Select(g => new TwoWayEventData
			{
				Bindings = g.ToList(),
			});

		var twoWayEventHandlers = twoWayEventHandlers2.Concat(twoWayEventHandlers3).ToList();
		for (int i = 0; i < twoWayEventHandlers.Count; i++)
		{
			twoWayEventHandlers[i].Index = i;
		}

		return new BindingsData
		{
			DataType = dataType,
			TargetType = targetType,
			Bindings = binds,
			NotifySources = notifySources,
			TwoWayEvents = twoWayEventHandlers,
			UpdateMethod = updateMethod,
		};

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

		UpdateMethodData CreateUpdateMethodData(IList<Bind> bindings, List<NotifySource>? notifySources, NotifySource? notifySource, Func<Expression, Expression> replace)
		{
			var notifySources1 = (notifySources ?? Enumerable.Empty<NotifySource>()).ToList();

			//Get the notify sources, for which UpdateXX methods are generated
			var notifySources2 = notifySources1
				.Where(s => s.Properties.Count > 1 && iNotifyPropertyChangedType.IsAssignableFrom(s.SourceExpression.Type))
				.OrderByDescending(s => s.Properties.SelectMany(p => p.Bindings).Distinct().Count())
				.ToList();

			// Find UpdateXX methods, which can be called from the main Update
			var updateNotifySources = new List<NotifySource>();
			while (notifySources2.Count > 0)
			{
				var s = notifySources2[0];
				updateNotifySources.Add(s);
				notifySources2.RemoveAt(0);

				// Remove also all child notify sources
				notifySources2 = notifySources2
					.Except(EnumerableExtensions.SelectTree(s, _ => _.Properties.SelectMany(_ => _.DependentNotifySources)), _ => _.Index)
					.ToList();
				notifySources1 = notifySources1
					.Except(EnumerableExtensions.SelectTree(s, _ => _.Properties.SelectMany(_ => _.DependentNotifySources)), _ => _.Index)
					.ToList();

				// Remove bindings set in this UpdateXX method
				s.Properties.SelectMany(_ => _.Bindings).ForEach(_ => bindings.Remove(_));
			}

			// Find UpdateXX_XX methods, which can be used in this Update
			var updateMethodNotifyProps = new List<NotifyProperty>();

			IEnumerable<NotifySource> notifySources3 = notifySources1;
			if (notifySource != null)
			{
				notifySources3 = notifySources3.Append(notifySource);
			}
			else
			{
				notifySources3 = notifySources1.Except(updateNotifySources);
			}

			var notifProps = notifySources3
				.SelectMany(d => d.Properties)
				.OrderByDescending(p => p.Bindings.Count)
				.ThenByDescending(p => p.DependentNotifySources.SelectTree(p2 => p2.Properties.SelectMany(p3 => p3.DependentNotifySources)).Count())
				.ToList();

			while (notifProps.Count > 0)
			{
				var prop = notifProps[0];
				updateMethodNotifyProps.Add(prop.Clone());
				prop.Bindings.ForEach(b => bindings.Remove(b));
				notifProps = notifProps.Where(p => !p.Bindings.Intersect(prop.Bindings).Any()).ToList();
				notifySources1 = notifySources1
					.Except(prop.DependentNotifySources.SelectTree(p => p.Properties.SelectMany(p2 => p2.DependentNotifySources)), f => f.Index)
					.ToList();
			}

			// Replace and group expressions of
			// - Bindings, set direct in this Update method
			// - UpdateXX methods
			// - UpdateXX_XX methods
			// - SetPropertyHandler methods

			var props1 = bindings
				.Select(b =>
				{
					var expr = b.SourceExpression!;
					var expr2 = replace(expr);
					if (expr is FallbackExpression && expr2 is not FallbackExpression && !taskType.IsAssignableFrom(expr.Type))
					{
						// The replaced expression is not FallbackExpression any more.
						// This means, that the part, which must be checked for nullability is replaced.
						// To solve it use full expression.
						expr2 = expr;
					}
					return new PropertySetExpression(b.Property, expr2);
				})
				.ToList();

			var props2 = updateNotifySources
				.Select(g => new PropertySetExpression(g.Properties[0].Bindings[0].Property, replace(g.Expression)))
				.ToList();

			var props3 = updateMethodNotifyProps
				.Select(p =>
				{
					var expr = replace(p.Expression);
					if (p.Property.PropertyType.Type.IsValueType &&
						!p.Property.PropertyType.Type.IsValueNullable() &&
						expr.IsNullable)
					{
						expr = new CoalesceExpression(expr, Expression.DefaultExpression);
					}
					return new PropertySetExpression(p.Bindings[0].Property, expr);
				})
				.ToList();

			var props4 = notifySources1
				.Select(g => new PropertySetExpression(g.Properties[0].Bindings[0].Property, replace(g.Expression)))
				.ToList();

			var props5 = props1.Concat(props2).Concat(props3).Concat(props4).ToList();

			var localVars2 = ExpressionUtils.GroupExpressions(props5);

			for (int i = 0; i < updateNotifySources.Count; i++)
			{
				updateNotifySources[i].SourceExpression = props2[i].Expression;
			}

			for (int i = 0; i < props3.Count; i++)
			{
				updateMethodNotifyProps[i].SourceExpression = props3[i].Expression;
			}

			for (int i = 0; i < props4.Count; i++)
			{
				notifySources1[i].SourceExpression = props4[i].Expression;
			}

			var updateExpressions = new ExpressionGroup
			{
				LocalVariables = localVars2,
				SetExpressions = props1
			};

			return new UpdateMethodData
			{
				UpdateNotifySources = updateNotifySources,
				UpdateNotifyProperties = updateMethodNotifyProps,
				Expressions = updateExpressions,
				SetEventHandlers = notifySources1,
			};
		}
	}

	private static class Res
	{
		public const string SyntaxError = "Syntax error.";
		public const string MissingExpression = "Missing expression.";
		public const string NoDataType = "DataType is unknown. It must be specified when using x:Bind in a DataTemplate.";
	}
}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BindingsData
{
	public TypeInfo? TargetType { get; init; }
	public TypeInfo DataType { get; init; }
	public IList<Bind> Bindings { get; init; }
	public List<NotifySource> NotifySources { get; init; }
	public List<TwoWayEventData> TwoWayEvents { get; init; }
	public UpdateMethodData UpdateMethod { get; init; }

	public void Validate(string file)
	{
		foreach (var binding in TwoWayEvents.SelectMany(b => b.Bindings))
		{
			if (binding.DependencyProperty == null && binding.TargetChangedEvent == null)
			{
				throw new GeneratorException($"Target change event cannot be determined. Set the event explicitly by setting the UpdateSourceTrigger property.", file, binding.Property.XamlNode);
			}
		}
	}
};

public class NotifySource
{
	public NotifyProperty Parent { get; set; }
	public Expression Expression { get; init; }
	public Expression SourceExpression { get; set; }
	public List<NotifyProperty> Properties { get; set; }
	public UpdateMethodData? UpdateMethod { get; set; }
	public int Index { get; set; }

	public NotifySource Clone()
	{
		return (NotifySource)MemberwiseClone();
	}
};

public class NotifyProperty
{
	public NotifySource Parent { get; init; }
	public PropertyInfo Property { get; init; }
	public Expression Expression { get; init; }
	public Expression SourceExpression { get; set; }
	public ReadOnlyCollection<Bind> Bindings { get; init; }
	public List<Bind> SetBindings { get; init; }
	public List<NotifySource> DependentNotifySources { get; } = new();
	public UpdateMethodData? UpdateMethod { get; set; }

	public NotifyProperty Clone()
	{
		return (NotifyProperty)MemberwiseClone();
	}
};

public class UpdateMethodData
{
	public ExpressionGroup Expressions { get; init; }
	public List<NotifySource> UpdateNotifySources { get; init; }
	public List<NotifyProperty> UpdateNotifyProperties { get; init; }
	public List<NotifySource> SetEventHandlers { get; init; }
}

public class TwoWayEventData
{
	public List<Bind> Bindings { get; init; }
	public int Index { get; set; }
};

public class Bind
{
	public XamlObjectProperty Property { get; init; }
	public Expression? Expression { get; init; }
	public Expression? BindBackExpression { get; init; }
	public BindingMode Mode { get; init; }
	public bool IsItemsSource { get; init; }
	public Expression? Converter { get; init; }
	public Expression? ConverterParameter { get; init; }
	public Expression? FallbackValue { get; init; }
	public TypeInfo? DataType { get; init; }
	public bool DataTypeSet { get; init; }
	public EventInfo? TargetChangedEvent { get; set; }
	public List<(string name, TypeInfo type)> Resources { get; init; }

	public Expression? SourceExpression { get; set; }

	public IMemberInfo? DependencyProperty;
	public int Index;
}

public enum BindingMode
{
	OneTime = 0x00,
	OneWay = 0x01,
	OneWayToSource = 0x10,
	TwoWay = 0x11,
}
