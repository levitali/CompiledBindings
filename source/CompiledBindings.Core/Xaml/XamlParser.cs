using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


#nullable enable

namespace CompiledBindings
{
	public class XamlParser
	{
		public static XamlNode ParseElement(string file, XElement element)
		{
			return ParseElement(file, element, element.Name.LocalName);
		}

		private static XamlNode ParseElement(string file, XElement element, string name)
		{
			var node = new XamlNode(file, element, element.Name.Namespace + name)
			{
				Value = element.Value
			};

			foreach (var attribute in element.Attributes().Where(a => a.Name != "xmlns" && a.Name.Namespace != XNamespace.Xmlns))
			{
				node.Properties.Add(ParseAttribute(file, attribute));
			}

			string propertyStartPart = node.Name.LocalName + ".";
			foreach (var child in element.Elements())
			{
				if (child.Name.LocalName.StartsWith(propertyStartPart))
				{
					node.Properties.Add(ParseElement(file, child, child.Name.LocalName.Substring(propertyStartPart.Length)));
				}
				else
				{
					node.Children.Add(ParseElement(file, child, child.Name.LocalName));
				}
			}

			return node;
		}

		public static XamlNode ParseAttribute(string file, XAttribute attribute)
		{
			var node = new XamlNode(file, attribute, attribute.Name);

			string str = attribute.Value.Trim();
			bool squareBracket = false;
			if (str.StartsWith("{") || (squareBracket = str.StartsWith("[")))
			{
				var expecedBracket = squareBracket ? "]" : "}";
				if (!str.EndsWith(expecedBracket))
				{
					throw new ParseException($"Expected {expecedBracket}", str.Length);
				}
				node.Children.Add(ParseMarkupExtension(attribute, file));
			}
			else
			{
				node.Value = str;
			}

			return node;
		}

		private static XamlNode ParseMarkupExtension(XAttribute attribute, string file)
		{
			return ParseMarkupExtension(attribute.Value.Trim(), attribute, file);
		}

		public static XamlNode ParseMarkupExtension(string str, XAttribute attribute, string file)
		{
			string? value;

			// Get the name
			int pos = str.IndexOf(' ', 1);
			if (pos == -1)
			{
				pos = str.Length - 1;
				value = null;
			}
			else
			{
				value = str.Substring(pos + 1, str.Length - pos - 2).Trim();
			}

			string name = str.Substring(1, pos - 1);
			var nodeName = GetTypeName(name, attribute.Parent);

			return new XamlNode(file, attribute, nodeName) { Value = value };
		}

		public static XName GetTypeName(string value, XObject xobject)
		{
			XNamespace ns;

			string prefix, className;
			var parts = value.Split(':');
			if (parts.Length > 2)
			{
				throw new ParseException($"Wrong syntax.");
			}
			else if (parts.Length == 2)
			{
				prefix = parts[0];
				className = parts[1];

				var nsAttr = EnumerableExtensions
					.SelectSequence(xobject, e => e.Parent, xobject is XElement)
					.Cast<XElement>()
					.SelectMany(e => e.Attributes())
					.FirstOrDefault(a => a.Name.Namespace == XNamespace.Xmlns && a.Name.LocalName == prefix);
				if (nsAttr == null)
				{
					throw new ParseException($"'{prefix}' is an undeclared prefix.");
				}
				ns = (XNamespace)nsAttr.Value;
			}
			else
			{
				className = parts[0];

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
		public string? Value;
		public List<XamlNode> Properties = new List<XamlNode>();
		public List<XamlNode> Children = new List<XamlNode>();

		public XamlNode(string file, XObject element, XName name)
		{
			File = file;
			Element = element;
			Name = name;
		}

		public string File { get; }
		public XObject Element { get; }
		public XName Name { get; }

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
			if (nsName.StartsWith("using:"))
			{
				return nsName.Substring("using:".Length);
			}
			else if (nsName.StartsWith("clr-namespace:"))
			{
				const int l = 14; // length of "clr-namespace:"
				int ind = nsName.IndexOf(';', l);
				if (ind == -1)
					ind = nsName.Length;
				return nsName.Substring(l, ind - l);
			}
			return null;
		}
	}
}
