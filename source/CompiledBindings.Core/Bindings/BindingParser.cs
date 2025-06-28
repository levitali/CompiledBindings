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
		string? stringFormat = null;
		bool dataTypeSet = false;
		BindingMode? mode = null;
		bool isItemsSource = false;
		List<EventInfo> targetChangedEvents = [];

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

					expr = new StaticResourceExpression(resourceName, resourceType);

					int pos2 = str.IndexOf(',');
					pos1 = pos2 == -1 ? (pos2 = str.Length) : pos2 + 1;
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
			else if (name is "StringFormat")
			{
				if (str.StartsWith("{}"))
				{
					str = str.Substring(2);
				}

				// Find end of string format. It can contain commas, escaped with \ symbol
				match = Regex.Match(str, @"^(.*?)(?<!\\),");
				if (match.Success)
				{
					stringFormat = match.Groups[1].Value;
					pos1 = stringFormat.Length + 1;
				}
				else
				{
					stringFormat = str;
					pos1 = stringFormat.Length;
				}
				stringFormat = stringFormat.Replace("\\,", ",");
			}
			else if (name is "Mode" or "UpdateSourceEventNames" or "DataType" or "IsItemsSource")
			{
				int pos2 = str.IndexOf(',');
				pos1 = pos2 == -1 ? (pos2 = str.Length) : pos2 + 1;

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

		if (mode is BindingMode.TwoWay or BindingMode.OneWayToSource &&
			(bindBackExpression ?? expression) is not (MemberExpression or CallExpression or ElementAccessExpression))
		{
			throw new ParseException("The expression must be settable for TwoWay or OneWayToSource bindings.");
		}

		if (isItemsSource && expression == null)
		{
			throw new ParseException("IsItemsSource cannot be used for OneWayToSource bindings.");
		}

		Expression? sourceExpression = expression;
		Expression? asyncSourceExpression = null;

		if (sourceExpression != null)
		{
			Expression sourceExpression2;

			var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));
			bool isTask = taskType.IsAssignableFrom(sourceExpression.Type);
			if (isTask)
			{
				var taskResultType = sourceExpression.Type.GetGenericArguments()![0];
				sourceExpression2 = new VariableExpression(taskResultType, "result");
			}
			else
			{
				sourceExpression2 = sourceExpression;
			}

			if (converter != null)
			{
				var convertMethod = xamlDomParser.ConverterType!.Methods.First(m => m.Definition.Name == "Convert");
				sourceExpression2 = new CallExpression(converter, convertMethod,
				[
					sourceExpression2,
					new TypeofExpression(new TypeExpression(prop.MemberType)),
					converterParameter ?? Expression.NullExpression,
					Expression.NullExpression
				]);
				if (prop.MemberType.Reference.FullName != "System.Object")
				{
					sourceExpression2 = new CastExpression(sourceExpression2, prop.MemberType, false);
				}
			}

			if (targetNullValue != null)
			{
				sourceExpression2 = new CoalesceExpression(sourceExpression2, targetNullValue);
			}

			if (stringFormat != null)
			{
				var stringFormatSaved = stringFormat;
				stringFormat = Regex.Replace(stringFormat, @"(.*)(\{)(0)(?:(\:.+))?(\})(.*)", "$\"$1{{{0}$4}}$6\"");
				if (stringFormat == stringFormatSaved)
				{
					stringFormat = $"$\"{{{{{{0}}:{stringFormat}}}}}\"";
				}
				sourceExpression2 = new InterpolatedStringExpression(stringFormat, new[] { sourceExpression2 });
			}

			if (isTask)
			{
				asyncSourceExpression = sourceExpression2;
			}
			else
			{
				if (fallbackValue != null)
				{
					sourceExpression2 = FallbackExpression.CreateFallbackExpression(sourceExpression2, fallbackValue, ref localVarIndex);
				}
				sourceExpression = sourceExpression2;
			}
		}

		return new Bind
		{
			SourceType = sourceType,
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
			SourceExpression = sourceExpression,
			AsyncSourceExpression = asyncSourceExpression,
		};
	}

	public static BindingsClass CreateBindingsClass(IList<Bind> binds, TypeInfo? targetType, TypeInfo dataType, TypeInfo? dependencyObjectType = null)
	{
		// Set unique indexes used as ids for all bindings in this binding scope
		for (int i = 0; i < binds.Count; i++)
		{
			binds[i].Index = i;
		}

		var iNotifyPropertyChangedType = TypeInfo.GetTypeThrow(typeof(INotifyPropertyChanged));
		var iNotifyCollectionChangedType = TypeInfo.GetTypeThrow(typeof(INotifyCollectionChanged));
		var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));

		// Go through all expressions in bindings and find notifiable properties, grouped by notifiable source
		var notifySources = GetNotifySources(binds, iNotifyPropertyChangedType, iNotifyCollectionChangedType, dependencyObjectType);

		// Set dependencies between notify sources
		foreach (var notifySource in notifySources.OrderByDescending(d => getSourceExpr(d.Expression).Key))
		{
			foreach (var prop in notifySource.Properties)
			{
				var expr = prop.Expression.Key;
				foreach (var notifPropData2 in notifySources
					.Where(g => g != notifySource && getSourceExpr(g.Expression).EnumerateTree().Any(e => e.Key.Equals(expr))))
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

		// Create Update methods for notify sources and properties
		foreach (var notifySource in notifySources)
		{
			// If the notify source has more than one property, the update method is needed
			// to update all properties for empty property name
			if (notifySource.ManyINotifyPropertyChangedProperties)
			{
				var bindings = notifySource.INotifyPropChangedProperties.SelectMany(_ => _.Bindings).Distinct().ToList();
				notifySource.UpdateMethod = CreateUpdateMethod(bindings, null, notifySource, notifySource.SourceExpression);
			}

			foreach (var prop in notifySource.Properties)
			{
				prop.UpdateMethod = CreateUpdateMethod(prop.SetBindings, prop.DependentNotifySources, null, notifySource.Expression);
			}
		}

		// Create the main Update method
		var binds1 = binds
			.Where(b => b.Property.TargetEvent == null && b.Mode != BindingMode.OneWayToSource)
			.ToList();
		var updateMethod = CreateUpdateMethod(binds1, notifySources, null, null);

		//*** Create data for two-way bindings

		var twoWayBinds = binds.Where(b => b.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource);

		// two-way bindings for which UpdateSourceEventNames are explicitely set
		var twoWayEventHandlers1 = twoWayBinds
			.Where(b => b.UpdateSourceEvents.Count > 0)
			.SelectMany(b => b.UpdateSourceEvents.Select(e => (bind: b, evnt: e)))
			.GroupBy(e => (e.bind, e.evnt.Signature))
			.Select(g => new TwoWayBinding
			{
				TargetChangedEvents = g.Select(_ => _.evnt).Distinct().ToList(),
				Bindings = g.Select(_ => _.bind).Distinct().ToList(),
			});

		// two-way bindings for dependency properties
		var twoWayEventHandlers2 = twoWayBinds
			.Where(b => b.UpdateSourceEvents.Count == 0 && b.DependencyProperty != null)
			.GroupBy(b => (b.Property.Object, b.DependencyProperty))
			.Select(g => new TwoWayBinding
			{
				Bindings = g.ToList(),
			});

		// two-way bindings for objects implementing INotifyPropertyChanged
		var twoWayEventHandlers3 = twoWayBinds
			.Where(b => b.UpdateSourceEvents.Count == 0 &&
						b.DependencyProperty == null &&
						iNotifyPropertyChangedType.IsAssignableFrom(b.Property.Object.Type))
			.GroupBy(b => b.Property.Object)
			.Select(g => new TwoWayBinding
			{
				TargetChangedEvents = [iNotifyPropertyChangedType.Events[0]],
				Bindings = g.ToList(),
			});

		var twoWayEventHandlers = twoWayEventHandlers1.Concat(twoWayEventHandlers2).Concat(twoWayEventHandlers3).ToList();
		for (int i = 0; i < twoWayEventHandlers.Count; i++)
		{
			twoWayEventHandlers[i].Index = i;
		}

		return new BindingsClass
		{
			DataType = dataType,
			TargetType = targetType,
			Bindings = binds,
			NotifySources = notifySources,
			TwoWayEvents = twoWayEventHandlers,
			UpdateMethod = updateMethod,
		};

		static Expression getSourceExpr(Expression expr)
		{
			if (expr is ParenExpression pe)
			{
				return getSourceExpr(pe.Expression);
			}
			else if (expr is CastExpression ce)
			{
				return getSourceExpr(ce.Expression);
			}
			return expr;
		}
	}

	public static List<NotifySource> GetNotifySources(IList<Bind> binds, TypeInfo iNotifyPropertyChangedType, TypeInfo iNotifyCollectionChangedType, TypeInfo? dependencyObjectType)
	{
		var notifySources = binds
			.Where(b => b.Property.TargetEvent == null &&
						b.SourceExpression != null &&
						b.Mode is not (BindingMode.OneTime or BindingMode.OneWayToSource))
			.SelectMany(b => b.SourceExpression!
							 .EnumerateTree()
							 .OfType<INotifiableExpression>()
							 .Select(e => (bind: b, expr: e, notif: checkPropertyNotifiable(e, out var isDependencyProp, out var isCollectionChanged), isDependencyProp, isCollectionChanged)))
			.Where(e => e.notif != false)
			.GroupBy(e => e.expr.Expression.Key)
			.OrderBy(g => g.Key)
			.Select((g, i) =>
			{
				var expr1 = g.First().expr.Expression;
				var d = new NotifySource
				{
					Expression = expr1,
					SourceExpression = expr1,
					IsINotifyPropertyChanged = iNotifyPropertyChangedType.IsAssignableFrom(expr1.Type),
					Index = i,
				};
				d.Properties = g
					.GroupBy(e => getMemberName(e.expr, e.isCollectionChanged))
					.Select(g2 =>
					{
						var (_, expr2, _, isDependencyProp, isCollectionChanged) = g2.First();
						var bindings = g2.Select(e => e.bind).Distinct().ToList();

						string propertyCodeName = g2.Key;
						var propertyNames = new List<string> { propertyCodeName };
						if (expr2 is ElementAccessExpression)
						{
							propertyNames.Add("Items[]");
						}

						return new NotifyProperty
						{
							Parent = d,
							Member = expr2.Member,
							PropertyCodeName = propertyCodeName,
							PropertyNames = propertyNames,
							IsDependencyProp = isDependencyProp,
							IsCollectionChangedElementAccess = isCollectionChanged,
							Expression = (Expression)expr2,
							SourceExpression = (Expression)expr2,
							Bindings = new ReadOnlyCollection<Bind>(bindings),
							SetBindings = bindings.ToList(), // Make copy
						};
					})
					.ToList();
				return d;

				static string getMemberName(INotifiableExpression expression, bool isCollectionChanged)
				{
					if (isCollectionChanged)
					{
						var expr = (ElementAccessExpression)expression;
						if (expr.Parameters.Length == 1 &&
							Expression.StripParenExpression(expr.Parameters[0]) is ConstantExpression ce)
						{
							return "Item" + ce.CSharpCode;
						}
					}
					return expression.Member?.Definition.Name ?? "Item";
				}
			})
			.ToList();

		return notifySources;

		bool? checkPropertyNotifiable(INotifiableExpression expr, out bool isDependencyProp, out bool isCollectionChanged)
		{
			isDependencyProp = false;
			isCollectionChanged = false;

			// Not notifiable if explicitly turned off with / operator
			if (expr.IsNotifiable == false)
			{
				return false;
			}

			// Check if type is notifiable
			var type = expr.Expression.Type;

			if (type.Reference.IsArray)
			{
				return false;
			}

			bool isDependencyType = dependencyObjectType?.IsAssignableFrom(type) == true;
			bool isNotifyPropertyChangedType = iNotifyPropertyChangedType.IsAssignableFrom(type);
			if (!isDependencyType && !isNotifyPropertyChangedType)
			{
				// Check if the type can be potentally notifiable
				if (!type!.Reference.IsInterface() && type.Definition?.IsSealed == true)
				{
					return false;
				}
			}

			if (expr is MemberExpression me)
			{
				// No notifications for static members
				// TODO maybe for NET 7 with abstract static interfaces
				if (me.Member.Definition.IsStatic())
				{
					return false;
				}

				// The \ operator overrides ReadOnlyAttribute and get-only properties
				if (expr.IsNotifiable == null)
				{
					// Check if ReadOnlyAttribute is set
					var attr = me.Member.Definition.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.ComponentModel.ReadOnlyAttribute");
					bool? readOnly = (bool?)attr?.ConstructorArguments[0].Value;
					if (readOnly == true)
					{
						return false;
					}

					// Check if the property is get-only
					else if (readOnly == null && me.Member is PropertyInfo pi && pi.IsReadOnly)
					{
						return false;
					}

					// Notifications for not properties (other members), must be explicitely enabled
					if (me.Member is not PropertyInfo)
					{
						return false;
					}

					// If the type is not notifiable and no \ operator is used
					if (!isNotifyPropertyChangedType && !isDependencyType)
					{
						return false;
					}
				}
				// If the type is not notifiable, but \ operator is used.
				else if (!isNotifyPropertyChangedType && !isDependencyType)
				{
					return null;
				}

				var dependencyPropertyName = me.Member.Definition.Name + "Property";
				isDependencyProp = isDependencyType && type.Fields.Cast<IMemberInfo>().Concat(type.Properties).Any(m => m.Definition.Name == dependencyPropertyName);

				// For dependency property check if there is the backing store field or property (WinCE/UWP).
				// It must be named <PropertyName>Property
				return isNotifyPropertyChangedType || isDependencyProp;
			}
			else if (expr is ElementAccessExpression ea && iNotifyCollectionChangedType.IsAssignableFrom(ea.Expression.Type))
			{
				isCollectionChanged = true;
				return true;
			}
			else
			{
				// Notifications for elment access are active only if explicitly enabled with \ operator
				if (expr.IsNotifiable == true)
				{
					return isNotifyPropertyChangedType ? true : null;
				}
			}
			return false;
		}
	}

	private static UpdateMethod CreateUpdateMethod(IList<Bind> bindings, List<NotifySource>? notifySources, NotifySource? notifySource, Expression? replacedExpression)
	{
		List<VariableExpression> parameters = [];

		VariableExpression? valueExpression = null;
		if (replacedExpression != null)
		{
			var type = ExpressionUtils.GetExpressionType(replacedExpression);
			valueExpression = new VariableExpression(type, "value");
			parameters.Add(valueExpression);
		}

		var notifySources1 = notifySources ?? [];

		//Get the notify sources, for which UpdateXX methods are generated,
		//ordered descending by number of bindings set in them.
		var notifySources2 = notifySources1
			.Where(s => s.ManyINotifyPropertyChangedProperties)
			.OrderByDescending(s => s.INotifyPropChangedProperties.SelectMany(p => p.Bindings).Distinct().Count())
			.ToList();

		// Get UpdateXX methods, which can be called from the main Update
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

		// Get UpdateXX_XX methods, which can be used in this Update
		var updateMethodNotifyProps = new List<NotifyProperty>();

		var notifySources3 = notifySource != null
			? notifySources1.Append(notifySource)
			: notifySources1.Except(updateNotifySources);

		var notifProps = notifySources3
			.SelectMany(d => d.Properties)
			.OrderByDescending(p => p.Bindings.Count)
			.ThenByDescending(p => p.DependentNotifySources.SelectTree(p2 => p2.Properties.SelectMany(p3 => p3.DependentNotifySources)).Count())
			.ToList();

		while (notifProps.Count > 0)
		{
			var prop = notifProps[0];
			updateMethodNotifyProps.Add(prop);
			prop.Bindings.ForEach(b => bindings.Remove(b));
			notifProps = notifProps.Where(p => !p.Bindings.Intersect(prop.Bindings).Any()).ToList();
			notifySources1 = notifySources1
				.Except(prop.DependentNotifySources.SelectTree(p => p.Properties.SelectMany(p2 => p2.DependentNotifySources)), f => f.Index)
				.ToList();
			foreach (var ns in prop.DependentNotifySources.SelectTree(p2 => p2.Properties.SelectMany(p3 => p3.DependentNotifySources)))
			{
				var index = updateNotifySources.IndexOf(s => s.Index == ns.Index);
				if (index != -1)
				{
					updateNotifySources.RemoveAt(index);
				}
			}
		}

		// Replace and group expressions of
		// - Bindings, set direct in this Update method
		// - UpdateXX methods
		// - UpdateXX_XX methods
		// - SetPropertyHandler methods

		var props1 = bindings
			.Select(b => new PropertySetData(b.Property, replace(b.SourceExpression!)))
			.ToList();

		var props2 = updateNotifySources
			.Select(g => new PropertySetData(g.Properties[0].Bindings[0].Property, replace(g.Expression)))
			.ToList();

		var props3 = updateMethodNotifyProps
			.Select(p => new PropertySetData(p.Bindings[0].Property, replace(p.Parent.Expression)))
			.ToList();

		var props4 = notifySources1
			.Select(g => new PropertySetData(g.Properties[0].Bindings[0].Property, replace(g.Expression)))
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

		return new UpdateMethod
		{
			Parameters = parameters,
			UpdateNotifySources = updateNotifySources,
			UpdateNotifyProperties = updateMethodNotifyProps,
			Expressions = updateExpressions,
			SetEventHandlers = notifySources1,
		};

		Expression replace(Expression expr)
		{
			if (replacedExpression != null)
			{
				expr = expr.CloneReplace(replacedExpression, valueExpression!);
			}
			return expr;
		}
	}

	private static class Res
	{
		public const string SyntaxError = "Syntax error.";
		public const string MissingExpression = "Missing expression.";
		public const string NoDataType = "DataType is unknown. It must be specified when using x:Bind in a DataTemplate.";
	}
}

public class BindingsClass
{
	public required TypeInfo? TargetType { get; init; }
	public required TypeInfo DataType { get; init; }
	public required IList<Bind> Bindings { get; init; }
	public required List<NotifySource> NotifySources { get; init; }
	public required List<TwoWayBinding> TwoWayEvents { get; init; }
	public required UpdateMethod UpdateMethod { get; init; }
};

public class NotifySource
{
	public required Expression Expression { get; init; }
	public required Expression SourceExpression { get; set; }
	public List<NotifyProperty> Properties { get; set; } = null!;
	public UpdateMethod? UpdateMethod { get; set; }
	public bool IsINotifyPropertyChanged { get; init; }
	public int Index { get; init; }

	public bool AnyDependencyProperty => Properties.Any(p => p.IsDependencyProp);

	public bool AnyCollectionChangedElementAccess => Properties.Any(p => p.IsCollectionChangedElementAccess);

	public bool AnyINotifyPropertyChangedProperty => Properties
		.Any(p => !p.IsDependencyProp && !p.IsCollectionChangedElementAccess);

	public bool ManyINotifyPropertyChangedProperties => INotifyPropChangedProperties.Take(2).Count() > 1;

	public IEnumerable<NotifyProperty> INotifyPropChangedProperties => Properties
		.Where(p => !p.IsDependencyProp && !p.IsCollectionChangedElementAccess);

	public NotifySource Clone()
	{
		return (NotifySource)MemberwiseClone();
	}
};

public class NotifyProperty
{
	public required NotifySource Parent { get; init; }
	public required IMemberInfo? Member { get; init; }
	public required bool IsDependencyProp { get; init; }
	public required bool IsCollectionChangedElementAccess { get; init; }
	public required IList<string> PropertyNames { get; init; }
	public required string PropertyCodeName { get; init; }
	public required Expression Expression { get; init; }
	public required Expression SourceExpression { get; set; }
	public required ReadOnlyCollection<Bind> Bindings { get; init; }
	public required List<Bind> SetBindings { get; init; }
	public List<NotifySource> DependentNotifySources { get; } = [];
	public UpdateMethod? UpdateMethod { get; set; }
};

public class UpdateMethod
{
	public required List<VariableExpression> Parameters { get; init; }
	public required ExpressionGroup Expressions { get; init; }
	public required List<NotifySource> UpdateNotifySources { get; init; }
	public required List<NotifyProperty> UpdateNotifyProperties { get; init; }
	public required List<NotifySource> SetEventHandlers { get; init; }
}

public class TwoWayBinding
{
	public required List<Bind> Bindings { get; init; }
	public List<EventInfo>? TargetChangedEvents { get; init; }
	public int Index { get; set; }
};

public class Bind
{
	public required TypeInfo SourceType { get; init; }
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

	// The final source expression,
	// including Converter, TargetNull, FallbackValue, StringFormat
	public Expression? SourceExpression { get; set; }
	public Expression? AsyncSourceExpression { get; set; }

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
