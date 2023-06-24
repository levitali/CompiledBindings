namespace CompiledBindings;

public class XamlParser
{
	private static readonly Regex _typeNameRegex = new(@"^(?:(\w+):)?(\w+)$");

	public static XamlNode ParseAttribute(string lineFile, XAttribute attribute, IList<XamlNamespace>? knownNamespaces)
	{
		var node = new XamlNode(lineFile, attribute, attribute.Name);

		var markupExtension = ParseMarkupExtension(lineFile, attribute, knownNamespaces);
		if (markupExtension != null)
		{
			node.Children.Add(markupExtension);
		}
		else
		{
			node.Value = attribute.Value;
		}

		return node;
	}

	private static XamlNode? ParseMarkupExtension(string lineFile, XAttribute attribute, IList<XamlNamespace>? knownNamespaces)
	{
		return ParseMarkupExtension(lineFile, attribute.Value.Trim(), attribute, knownNamespaces);
	}

	public static XamlNode? ParseMarkupExtension(string lineFile, string str, XAttribute attribute, IList<XamlNamespace>? knownNamespaces)
	{
		var match = Regex.Match(str, @"^\s*([{\[])\s*((?:\w+:)?\w+)(?:(\s+.+?))?\s*([}\]])\s*$");
		if (!match.Success)
		{
			return null;
		}

		var expectedEndBracket = match.Groups[1].Value == "{" ? "}" : "]";
		if (match.Groups[4].Value != expectedEndBracket)
		{
			throw new ParseException($"Expected {expectedEndBracket}", match.Groups[4].Length);
		}

		var name = match.Groups[2].Value;
		var value = match.Groups[3].Value;
		int valueOffset = match.Groups[3].Index;

		var nodeName = GetTypeName(name, attribute.Parent, knownNamespaces);

		return new XamlNode(lineFile, attribute, nodeName) { Value = value, ValueOffset = valueOffset };
	}

	public static XName GetTypeName(string value, XObject xobject, IList<XamlNamespace>? knownNamespaces)
	{
		XNamespace ns;

		var match = _typeNameRegex.Match(value);
		if (!match.Success)
		{
			throw new ParseException($"Wrong syntax.");
		}
		var prefix = match.Groups[1].Value;
		var className = match.Groups[2].Value;
		if (!string.IsNullOrEmpty(prefix))
		{
			var nsAttr = EnumerableExtensions
				.SelectSequence(xobject, e => e.Parent, xobject is XElement)
				.Cast<XElement>()
				.SelectMany(e => e.Attributes())
				.FirstOrDefault(a => a.Name.Namespace == XNamespace.Xmlns && a.Name.LocalName == prefix);
			if (nsAttr == null)
			{
				var ns2 = knownNamespaces?.FirstOrDefault(n => n.Prefix == prefix);
				if (ns2 == null)
				{
					throw new ParseException($"'{prefix}' is an undeclared prefix.");
				}
				ns = ns2.Namespace;
			}
			else
			{
				ns = nsAttr.Value;
			}
		}
		else
		{
			var nsAttr = EnumerableExtensions
				.SelectSequence(xobject, e => e.Parent, xobject is XElement)
				.Cast<XElement>()
				.SelectMany(e => e.Attributes())
				.FirstOrDefault(a => a.Name.Namespace == XNamespace.None && a.Name.LocalName == "xmlns");
			ns = (XNamespace)nsAttr?.Value ?? XNamespace.None;
		}

		return ns + className;
	}
}

public class XamlNode
{
	private List<XamlNode>? _properties;
	private List<XamlNode>? _children;

	public XamlNode(string lineFile, XObject element, XName name)
	{
		LineFile = lineFile;
		Element = element;
		Name = name;
	}

	public string LineFile { get; }
	public XObject Element { get; }
	public XName Name { get; }

	public List<XamlNode> Properties => _properties ??= new List<XamlNode>();

	public List<XamlNode> Children => _children ??= new List<XamlNode>();

	public string? Value { get; set; }
	public int ValueOffset { get; set; }

	public void Remove()
	{
		if (Element is XElement xe)
		{
			xe.Remove();
		}
		else
		{
			((XAttribute)Element).Remove();
		}
	}
}

public class XamlNamespace
{
	private static readonly Regex _usingRegex = new(@"^\s*(?:(global\s+))?using\s*:(.+)$");
	private static readonly Regex _clrNamespaceRegex = new(@"^clr-namespace:(.+?)(?:;.+)?$");

	public XamlNamespace(string prefix, XNamespace ns)
	{
		Prefix = prefix;
		Namespace = ns;
		ClrNamespace = GetClrNamespace(ns.NamespaceName);
	}

	public string Prefix { get; }
	public XNamespace Namespace { get; }
	public string? ClrNamespace { get; }

	public static string? GetClrNamespace(string nsName)
	{
		var match = _usingRegex.Match(nsName);
		if (match.Success)
		{
			return match.Groups[match.Groups.Count - 1].Value.Trim();
		}
		if ((match = _clrNamespaceRegex.Match(nsName)).Success)
		{
			return match.Groups[1].Value.Trim();
		}
		return null;
	}

	public static IEnumerable<XamlNamespace> GetClrNamespaces(XObject xobject)
	{
		var xelement = xobject as XElement ?? xobject.Parent;
		return EnumerableExtensions
			.SelectSequence(xelement, e => e.Parent, true)
			.SelectMany(e => e.Attributes())
			.Where(a => a.Name.Namespace == XNamespace.Xmlns)
			.Select(a => new XamlNamespace(a.Name.LocalName, a.Value))
			.Where(n => n.ClrNamespace != null);
	}

	public static IList<XamlNamespace> GetGlobalNamespaces(IEnumerable<XDocument> xdocs)
	{
		return xdocs
			.SelectMany(xdoc => xdoc.Descendants().Attributes())
			.Where(a => a.Name.Namespace == XNamespace.Xmlns)
			.Select(a => (a, m: _usingRegex.Match(a.Value)))
			.Where(e => e.m.Success && !string.IsNullOrEmpty(e.m.Groups[1].Value))
			.Select(e => new XamlNamespace(e.a.Name.LocalName, e.a.Value.Trim()))
			.Distinct(n => n.Prefix)
			.ToList();
	}
}
