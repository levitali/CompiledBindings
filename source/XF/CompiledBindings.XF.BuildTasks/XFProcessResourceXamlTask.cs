using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace CompiledBindings;

public class XFProcessResourceXamlTask : Task
{
	private readonly PlatformConstants _platformConstants;

	public XFProcessResourceXamlTask() : this(new PlatformConstants())
	{
	}

	public XFProcessResourceXamlTask(PlatformConstants platformConstants)
	{
		_platformConstants = platformConstants;
	}

	[Required]
	public required ITaskItem[] ReferenceAssemblies { get; init; }

	[Required]
	public required string Assembly { get; init; }

	public bool AttachDebugger { get; set; }

	public override bool Execute()
	{
		try
		{
			if (AttachDebugger)
			{
				System.Diagnostics.Debugger.Launch();
			}

			TypeInfoUtils.LoadReferences(ReferenceAssemblies.Select(a => a.ItemSpec));

			var prm = new ReaderParameters(ReadingMode.Immediate)
			{
				ReadWrite = true,
				ReadSymbols = true,
				AssemblyResolver = new TypeInfoUtils.AssemblyResolver(),
			};

			using var assembly = AssemblyDefinition.ReadAssembly(Assembly, prm);

			var helperTypeAssembly = TypeInfoUtils.Assemblies
				.FirstOrDefault(a => a.MainModule.GetAllTypes().Any(t => t.FullName == $"CompiledBindings.{_platformConstants.FrameworkId}.CompiledBindingsHelper"))?
				.Name.Name;

			var assemblyTypes = assembly.MainModule.Types.ToDictionary(_ => _.FullName);

			var xamlDomParser = new XFXamlDomParser(_platformConstants);

			bool assemblyModified = false;
			foreach (var module in assembly.Modules)
			{
				List<(EmbeddedResource oldResource, EmbeddedResource newResource)> replaceResources = new();
				foreach (var resource in module.Resources.OfType<EmbeddedResource>())
				{
					if (!resource.Name.EndsWith(".xaml", StringComparison.InvariantCulture))
					{
						continue;
					}

					string xaml;
					using (var resourceStream = resource.GetResourceStream())
					using (var streamReader = new StreamReader(resourceStream))
					{
						xaml = streamReader.ReadToEnd();
					}

					var newXaml = XFXamlProcessor.ProcessXaml(xaml, xamlDomParser, _platformConstants, assemblyTypes, helperTypeAssembly);
					if (newXaml != null)
					{
						var newResource = new EmbeddedResource(resource.Name, resource.Attributes, Encoding.UTF8.GetBytes(newXaml));
						replaceResources.Add((resource, newResource));
					}
				}

				if (replaceResources.Count > 0)
				{
					foreach (var (oldResource, newResource) in replaceResources)
					{
						module.Resources.Remove(oldResource);
						module.Resources.Add(newResource);
					}
					assemblyModified |= true;
				}
			}

			if (assemblyModified)
			{
				assembly.Write(new WriterParameters { WriteSymbols = true });
			}

			return true;
		}
		catch (GeneratorException ex)
		{
			Log.LogError(null, null, null, ex.File, ex.LineNumber, ex.ColumnNumber, ex.EndLineNumber, ex.EndColumnNumber, ex.Message);
			return false;
		}
		catch (Exception ex)
		{
			Log.LogError(ex.Message);
			return false;
		}
		finally
		{
			TypeInfoUtils.Cleanup();
		}
	}
}

public static class XFXamlProcessor
{
	public static string? ProcessXaml(string xaml, XFXamlDomParser xamlDomParser, PlatformConstants platformConstants, Dictionary<string, TypeDefinition> assemblyTypes, string? helperTypeAssembly)
	{
		XDocument xdoc;
		try
		{
			xdoc = XDocument.Parse(xaml, LoadOptions.SetLineInfo);
		}
		catch
		{
			return null;
		}

		var xclass = xdoc.Root.Attribute(xamlDomParser.xClass);
		if (xclass != null)
		{
			var mAttrs = xdoc.Descendants().SelectMany(e => e.Attributes()).Where(a => xamlDomParser.IsMemExtension(a) != null).ToList();
			var mxAttrs = xdoc.Descendants().SelectMany(e => e.Attributes()).Where(a => a.Name.Namespace == SimpleXamlDomParser.mxNamespace).ToList();
			if (mAttrs.Count > 0 || mxAttrs.Count > 0)
			{
				// Note! Here XAML is modified, but not with XDocument.
				// XDocument on save breaks original formatting completely.
				// It is important to keep original formatting, because there can
				// be errors in XAML.
				var lines = new List<TextLine>();
				var stringReader = new StringReader(xaml);
				string ln;
				while ((ln = stringReader.ReadLine()) != null)
				{
					lines.Add(new TextLine { Text = ln });
				}

				int nsIndex = 0;

				if (mAttrs.Count > 0)
				{
					var parts = xclass.Value.Split('.');
					string className = parts[parts.Length - 1];
					string classNs = string.Join(".", parts.Take(parts.Length - 1));
					string? compiledBindingsNsPrefix = null, classNsPrefix = null;

					var usedNames = new HashSet<string>(xdoc.Descendants().Select(e => e.Attribute(xamlDomParser.xName)).Where(a => a != null).Select(a => a.Value).Distinct());
					var elementNames = new Dictionary<XElement, string>();
					foreach (var xelement in xdoc.Descendants())
					{
						var xname = xelement.Attribute(xamlDomParser.xName);
						if (xname == null)
						{
							if (xelement != xdoc.Root &&
								(xelement.Attributes().Any(a => xamlDomParser.IsMemExtension(a) != null) ||
								 ((xelement.Attribute(xamlDomParser.xDataType) != null || xelement.Attribute(xamlDomParser.ControlTemplate) != null) && xelement.Name != xamlDomParser.DataTemplate)))
							{
								var name = XamlDomParser.GenerateName(xelement, usedNames);
								insertAtEnd(xelement, $" x:Name=\"{name}\"");

								elementNames.Add(xelement, name);
							}
						}
						else
						{
							elementNames.Add(xelement, xname.Value);
						}
					}

					// Process DataTemplates
					var dataTemplates = xdoc.Descendants().Where(x => x.Name == xamlDomParser.DataTemplate || x.Name == xamlDomParser.ControlTemplate).ToList();
					for (int i = 0; i < dataTemplates.Count; i++)
					{
						var dataTemplate = dataTemplates[i];
						var memExtensions = dataTemplate.Descendants()
							.SelectMany(e => e.Attributes())
							.Where(a =>
								xamlDomParser.IsMemExtension(a) != null &&
								a.Parent.Ancestors().Where(a => a.Name == xamlDomParser.DataTemplate || a.Name == xamlDomParser.ControlTemplate).First() == dataTemplate)
							.ToList();
						if (memExtensions.Count == 0)
						{
							continue;
						}

						var dateTemplateClassName = $"{className}_DataTemplate{i}";
						var dateTemplateFullClassName = $"{classNs}.{dateTemplateClassName}";
						if (!assemblyTypes.TryGetValue(dateTemplateFullClassName, out var dataTemplateType))
						{
							continue;
						}

						if (compiledBindingsNsPrefix == null)
						{
							compiledBindingsNsPrefix = searchNsPrefix($"CompiledBindings.{platformConstants.FrameworkId}", helperTypeAssembly);
							classNsPrefix = searchNsPrefix(classNs, null);
						}

						var propInitializers = string.Join(", ", dataTemplateType.Properties.Select(p => $"{p.Name}={{StaticResource {p.Name}}}"));

						var rootElement = dataTemplate.Elements().First();
						insertAtEnd(rootElement, $" {compiledBindingsNsPrefix}:CompiledBindingsHelper.Bindings=\"{{{classNsPrefix}:{dateTemplateClassName} {propInitializers}}}\"");
					}

					// Replace/Remove attributes with CompiledBindings namespace
					foreach (var attr in mAttrs.Concat(mxAttrs))
					{
						var lineInfo = (IXmlLineInfo)attr;
						var startLine = lineInfo.LineNumber - 1;
						var textLine = lines[startLine];
						var startPos = lineInfo.LinePosition - 1 - textLine.RemovedTextOffset;
						Debug.Assert(startPos >= 0);
						var (endLine, endPos) = getAttributeEnd(attr);

						if (elementNames.TryGetValue(attr.Parent, out string name))
						{
							var markupExtensionName = className;
							var dataTemplate = attr.Parent.Ancestors(xamlDomParser.DataTemplate).FirstOrDefault();
							if (dataTemplate != null)
							{
								int index = dataTemplates.IndexOf(dataTemplate);
								markupExtensionName += $"_DataTemplate{index}";
							}
							markupExtensionName += $"_{name}_{attr.Name.LocalName}";
							var markupExtensionFullName = $"{classNs}.{markupExtensionName}";
							if (assemblyTypes.ContainsKey(markupExtensionFullName))
							{
								// Replace the attribute with markup extension

								if (classNsPrefix == null)
								{
									classNsPrefix = searchNsPrefix(classNs, null);
								}

								int nameEnd = textLine.Text.IndexOf('=', startPos);
								string attrCont = textLine.Text.Substring(startPos, nameEnd - startPos + 1);
								attrCont += $"\"{{{classNsPrefix}:{markupExtensionName}}}\"";

								if (startLine == endLine)
								{
									Debug.Assert(endPos > startPos);
									var length = endPos - startPos;
									textLine.Text = textLine.Text.Substring(0, startPos) + attrCont + textLine.Text.Substring(endPos);
									textLine.RemovedTextOffset += length - attrCont.Length;
								}
								else
								{
									textLine.RemovedTextOffset += textLine.Text.Length - startPos;
									textLine.Text = textLine.Text.Substring(0, startPos) + attrCont;
									removeNextLines();
								}

								continue;
							}
						}

						// Remove the attribute
						if (startLine == endLine)
						{
							Debug.Assert(endPos > startPos);
							var length = endPos - startPos;
							textLine.Text = textLine.Text.Remove(startPos, length);
							textLine.RemovedTextOffset += length;
						}
						else
						{
							textLine.RemovedTextOffset += textLine.Text.Length - startPos;
							textLine.Text = textLine.Text.Substring(0, startPos);
							removeNextLines();
						}

						attr.Remove();

						void removeNextLines()
						{
							for (int i = startLine + 1; i < endLine; i++)
							{
								textLine = lines[i];
								textLine.Text = "";
								textLine.RemovedTextOffset += textLine.Text.Length;
							}
							textLine = lines[endLine];
							textLine.Text = textLine.Text.Substring(endPos);
							textLine.RemovedTextOffset = endPos;
						}
					}
				}

				return string.Join("\n", lines.Select(l => l.Text));
				
				string searchNsPrefix(string clrNs, string? assembly)
				{
					var searchedUsingNs = "using:" + clrNs;
					var searchedClrNs = "clr-namespace:" + clrNs;
					string prefix;

					var attr = xdoc.Root.Attributes().FirstOrDefault(a =>
						a.Name.Namespace == XNamespace.Xmlns &&
						(a.Value == searchedUsingNs || a.Value == searchedClrNs));
					if (attr != null)
					{
						prefix = attr.Name.LocalName;
					}
					else
					{
						do
						{
							prefix = "g" + nsIndex++;
						}
						while (xdoc.Root.Attributes().Any(a =>
							a.Name.Namespace == XNamespace.Xmlns && a.Name.LocalName == prefix));

						var ns = $"clr-namespace:{clrNs}";
						if (assembly != null)
						{
							ns += ";assembly=" + assembly;
						}
						insertAtEnd(xdoc.Root, $" xmlns:{prefix}=\"{ns}\"");
					}

					return prefix;
				}

				void insertAtEnd(XElement xelement, string text)
				{
					var lineInfo = (IXmlLineInfo)xelement;

					int lineNumber, startPos;
					var attr = xelement.LastAttribute;
					if (attr == null)
					{
						lineNumber = lineInfo.LineNumber - 1;
						startPos = lineInfo.LinePosition - 1 + xelement.Name.LocalName.Length;
					}
					else
					{
						(lineNumber, startPos) = getAttributeEnd(attr);
					}

					var textLine = lines[lineNumber];
					textLine.Text = textLine.Text.Insert(startPos, text);
				}

				(int line, int col) getAttributeEnd(XAttribute attr)
				{
					var lineInfo = (IXmlLineInfo)attr;
					var line = lineInfo.LineNumber - 1;
					var textLine = lines[line];
					var col = lineInfo.LinePosition - 1 - textLine.RemovedTextOffset;
					Debug.Assert(col >= 0);
					var text = textLine.Text;
					searchChar('=');
					searchChar('"');
					searchChar('"');
					return (line, col);

					void searchChar(char ch)
					{
						while (true)
						{
							col = text.IndexOf(ch, col);
							if (col != -1)
							{
								col++;
								return;
							}

							line++;
							textLine = lines[line];
							text = textLine.Text;
							col = 0;
						}
					}
				}
			}
		}

		return null;
	}

	private class TextLine
	{
		public required string Text;
		public int RemovedTextOffset;
	}
}

