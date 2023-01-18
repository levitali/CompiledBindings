namespace CompiledBindings;

public static class BindingParser
{
	public static Bind Parse(XamlObjectProperty prop, TypeInfo sourceType, TypeInfo targetType, string dataRootName, BindingMode defaultBindMode, XamlDomParser xamlDomParser, HashSet<string> includeNamespaces, bool throwIfWithoutDataType, ref int localVarIndex)
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
		List<EventInfo> targetChangedEvents = new();
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
					resources.Add((resourceName, resourceType!));

					var resourceField = new FieldInfo(new FieldDefinition(resourceName, FieldAttributes.Private, resourceType!.Reference), resourceType);
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
					IList<XamlNamespace> includeNamespaces2;
					try
					{
						expr = ExpressionParser.Parse(sourceType, dataRootName, str, prop.MemberType, false, namespaces, out includeNamespaces2, out pos1);
					}
					catch (ParseException ex)
					{
						throw new ParseException(ex.Message, currentPos + ex.Position, ex.Length);
					}
					includeNamespaces.UnionWith(includeNamespaces2.Select(n => n.ClrNamespace!));
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
			else if (name is "Mode" or "UpdateSourceEventNames" or "DataType" or "IsItemsSource")
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
					foreach (var eventName in value.Split('|').Select(_ => _.Trim()).Distinct())
					{
						var targetChangedEvent = prop.Object.Type.Events.FirstOrDefault(e => e.Definition.Name == eventName);
						if (targetChangedEvent == null)
						{
							throw new ParseException($"The type {prop.Object.Type.Reference.FullName} does not have event {value}.", currentPos + match.Groups[2].Index);
						}
						targetChangedEvents.Add(targetChangedEvent);
					}
					mode ??= BindingMode.TwoWay;
				}
			}
			else
			{
				throw new ParseException($"Property {name} is not valid for x:Bind", currentPos);
			}
		}

		if (expression == null)
		{
			if (bindBackExpression == null)
			{
				throw new ParseException("Missing Path or BindBack expression.");
			}
			if (mode == null)
			{
				mode = BindingMode.OneWayToSource;
			}
			else if (mode != BindingMode.OneWayToSource)
			{
				throw new ParseException("Missing expression.");
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

		var sourceExpression = expression;

		if (sourceExpression != null && converter != null)
		{
			var convertMethod = xamlDomParser.ConverterType!.Methods.First(m => m.Definition.Name == "Convert");
			sourceExpression = new CallExpression(converter, convertMethod, new Expression[]
			{
				sourceExpression,
				new TypeofExpression(new TypeExpression(prop.MemberType)),
				converterParameter ?? Expression.NullExpression,
				Expression.NullExpression
			});
			if (prop.MemberType.Reference.FullName != "System.Object")
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
			UpdateSourceEvents = targetChangedEvents,
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
		var notifySources = GetNotifySources(binds, iNotifyPropertyChangedType, dependencyObjectType);

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
				notifySource.UpdateMethod = CreateUpdateMethodData(bindings, null, notifySource, notifySource.SourceExpression);
			}

			foreach (var prop in notifySource.Properties)
			{
				prop.UpdateMethod = CreateUpdateMethodData(prop.SetBindings, prop.DependentNotifySources, null, prop.Expression);
			}
		}

		var binds1 = binds
			.Where(b => b.Property.TargetEvent == null && b.Mode != BindingMode.OneWayToSource)
			.ToList();
		var updateMethod = CreateUpdateMethodData(binds1, notifySources, null, null);

		var twoWayBinds = binds.Where(b => b.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource);

		var twoWayEventHandlers1 = twoWayBinds
			.Where(b => b.UpdateSourceEvents.Count > 0)
			.SelectMany(b => b.UpdateSourceEvents.Select(e => (bind: b, evnt: e)))
			.GroupBy(e => (e.bind.Property.Object, e.evnt.Signature))
			.Select(g => new TwoWayEventData
			{
				Events = g.Select(_ => _.evnt).Distinct().ToList(),
				Bindings = g.Select(_ => _.bind).Distinct().ToList(),
			});

		var twoWayEventHandlers2 = twoWayBinds
			.Where(b => b.UpdateSourceEvents.Count == 0 && b.DependencyProperty != null)
			.GroupBy(b => (b.Property.Object, b.DependencyProperty))
			.Select(g => new TwoWayEventData
			{
				Bindings = g.ToList(),
			});

		var twoWayEventHandlers = twoWayEventHandlers1.Concat(twoWayEventHandlers2).ToList();
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

		UpdateMethodData CreateUpdateMethodData(IList<Bind> bindings, List<NotifySource>? notifySources, NotifySource? notifySource, Expression? replacedExpression)
		{
			List<VariableExpression> parameters = new();

			VariableExpression? valueExpression = null;
			if (replacedExpression != null)
			{
				var type = ExpressionUtils.GetExpressionType(replacedExpression);
				valueExpression = new VariableExpression(type, "value");
				parameters.Add(valueExpression);
			}

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
					var expr2 = Replace(expr);
					if (expr.EnumerateTree().OfType<FallbackExpression>().Any() &&
						!expr2.EnumerateTree().OfType<FallbackExpression>().Any() &&
						!taskType.IsAssignableFrom(expr.Type))
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
				.Select(g => new PropertySetExpression(g.Properties[0].Bindings[0].Property, Replace(g.Expression)))
				.ToList();

			var props3 = updateMethodNotifyProps
				.Select(p =>
				{
					var expr = Replace(p.Expression);
					return new PropertySetExpression(p.Bindings[0].Property, expr);
				})
				.ToList();

			var props4 = notifySources1
				.Select(g => new PropertySetExpression(g.Properties[0].Bindings[0].Property, Replace(g.Expression)))
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
				Parameters = parameters,
				UpdateNotifySources = updateNotifySources,
				UpdateNotifyProperties = updateMethodNotifyProps,
				Expressions = updateExpressions,
				SetEventHandlers = notifySources1,
			};

			Expression Replace(Expression expr)
			{
				if (replacedExpression != null)
				{
					expr = expr.CloneReplace(replacedExpression, valueExpression!);
				}
				return expr;
			}
		}
	}

	public static List<NotifySource> GetNotifySources(IList<Bind> binds, TypeInfo iNotifyPropertyChangedType, TypeInfo? dependencyObjectType)
	{
		var notifySources = binds
			.Where(b => b.Property.TargetEvent == null &&
						b.SourceExpression != null &&
						b.Mode is not (BindingMode.OneTime or BindingMode.OneWayToSource))
			.SelectMany(b => b.SourceExpression!.EnumerateTree().OfType<MemberExpression>().Select(e => (bind: b, expr: e, notif: CheckPropertyNotifiable(e))))
			.Where(e => e.notif != false)
			.GroupBy(e => e.expr.Expression.CSharpCode)
			.OrderBy(g => g.Key)
			.Select((g, i) =>
			{
				var f = g.First();
				var expr1 = f.expr.Expression;
				var d = new NotifySource
				{
					Expression = expr1,
					SourceExpression = expr1,
					CheckINotifyPropertyChanged = f.notif == null,
					Index = i,
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
			.ToList();

		bool? CheckPropertyNotifiable(MemberExpression expr)
		{
			if (expr.IsNotifiable == false)
			{
				return false;
			}
			var type = expr.Expression.Type;
			var res = expr.Member is PropertyInfo pi &&
				   !pi.Definition.IsStatic() &&
				   (iNotifyPropertyChangedType.IsAssignableFrom(type) ||
				   (dependencyObjectType?.IsAssignableFrom(type) == true && 
						type.Fields.Cast<IMemberInfo>().Concat(type.Properties).Any(m => m.Definition.Name == pi.Definition.Name + "Property"))) &&
				   !pi.IsReadOnly;
			if (res)
				return true;
			if (expr.IsNotifiable == true)
				return null;
			return false;
		}

		return notifySources;
	}

	private static class Res
	{
		public const string SyntaxError = "Syntax error.";
		public const string MissingExpression = "Missing expression.";
		public const string NoDataType = "DataType is unknown. It must be specified when using x:Bind in a DataTemplate.";
	}
}

public class BindingsData
{
	public required TypeInfo? TargetType { get; init; }
	public required TypeInfo DataType { get; init; }
	public required IList<Bind> Bindings { get; init; }
	public required List<NotifySource> NotifySources { get; init; }
	public required List<TwoWayEventData> TwoWayEvents { get; init; }
	public required UpdateMethodData UpdateMethod { get; init; }
};

public class NotifySource
{
	public required Expression Expression { get; init; }
	public required Expression SourceExpression { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public List<NotifyProperty> Properties { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public UpdateMethodData? UpdateMethod { get; set; }
	public bool CheckINotifyPropertyChanged { get; init; }
	public int Index { get; init; }

	public NotifySource Clone()
	{
		return (NotifySource)MemberwiseClone();
	}
};

public class NotifyProperty
{
	public required NotifySource Parent { get; init; }
	public required PropertyInfo Property { get; init; }
	public required Expression Expression { get; init; }
	public required Expression SourceExpression { get; set; }
	public required ReadOnlyCollection<Bind> Bindings { get; init; }
	public required List<Bind> SetBindings { get; init; }
	public List<NotifySource> DependentNotifySources { get; } = new();
	public UpdateMethodData? UpdateMethod { get; set; }

	public NotifyProperty Clone()
	{
		return (NotifyProperty)MemberwiseClone();
	}
};

public class UpdateMethodData
{
	public required List<VariableExpression> Parameters { get; init; }
	public required ExpressionGroup Expressions { get; init; }
	public required List<NotifySource> UpdateNotifySources { get; init; }
	public required List<NotifyProperty> UpdateNotifyProperties { get; init; }
	public required List<NotifySource> SetEventHandlers { get; init; }
} 

public class TwoWayEventData
{
	public required List<Bind> Bindings { get; init; }
	public List<EventInfo>? Events { get; init; }
	public int Index { get; set; }
};

public class Bind
{
	public required XamlObjectProperty Property { get; init; }
	public Expression? Expression { get; init; }
	public Expression? BindBackExpression { get; init; }
	public BindingMode Mode { get; init; }
	public bool IsItemsSource { get; init; }
	public Expression? Converter { get; init; }
	public Expression? ConverterParameter { get; init; }
	public Expression? FallbackValue { get; init; }
	public TypeInfo? DataType { get; init; }
	public bool DataTypeSet { get; init; }
	public required List<EventInfo> UpdateSourceEvents { get; init; }
	public required List<(string name, TypeInfo type)> Resources { get; init; }

	public Expression? SourceExpression { get; set; }

	public IMemberInfo? DependencyProperty { get; set; }
	public int Index { get; set; }
}

public enum BindingMode
{
	OneTime = 0x00,
	OneWay = 0x01,
	OneWayToSource = 0x10,
	TwoWay = 0x11,
}
