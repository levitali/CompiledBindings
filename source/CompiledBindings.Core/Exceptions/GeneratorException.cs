#nullable enable

namespace CompiledBindings;

public class GeneratorException : Exception
{
	public GeneratorException(string message, string file, int lineNumber, int columnNumber, int length) : base(message)
	{
		File = file;
		LineNumber = lineNumber;
		ColumnNumber = columnNumber;
		EndLineNumber = lineNumber;
		EndColumnNumber = columnNumber + length;
	}

	public GeneratorException(string message, SequencePoint? sequencePoint) : base(message)
	{
		if (sequencePoint != null)
		{
			File = sequencePoint.Document.Url;
			LineNumber = sequencePoint.StartLine;
			ColumnNumber = sequencePoint.StartColumn;
			EndLineNumber = sequencePoint.EndLine;
			EndColumnNumber = sequencePoint.EndColumn;
		}
	}

	public GeneratorException(string message, string file, XObject xnode, int offset = 0) : base(message)
	{
		File = file;
		if (xnode is IXmlLineInfo lineInfo)
		{
			LineNumber = lineInfo.LineNumber;
			ColumnNumber = lineInfo.LinePosition;
		}
		ColumnNumber += offset;
	}

	public GeneratorException(string message, XamlNode xamlNode, int offset = 0) : this(message, xamlNode.File, xamlNode.Element, offset)
	{
	}

	public GeneratorException(string message, LineInfo lineInfo) : base(message)
	{
		if (lineInfo != null)
		{
			File = lineInfo.File;
			LineNumber = lineInfo.LineNumber;
			ColumnNumber = lineInfo.ColumnNumber;
			EndLineNumber = lineInfo.EndLineNumber;
			EndColumnNumber = lineInfo.EndColumnNumber;
		}
	}

	public string? File { get; set; }

	public int LineNumber { get; private set; }

	public int ColumnNumber { get; private set; }

	public int EndLineNumber { get; private set; }

	public int EndColumnNumber { get; private set; }
}

public class LineInfo
{
	public LineInfo()
	{
	}

	public LineInfo(string file, XObject xnode)
	{
		File = file;
		if (xnode is IXmlLineInfo lineInfo)
		{
			LineNumber = lineInfo.LineNumber;
			ColumnNumber = lineInfo.LinePosition;
		}
	}

	public LineInfo(string file, XObject xnode, int offset)
	{
		File = file;
		if (xnode is IXmlLineInfo lineInfo)
		{
			LineNumber = lineInfo.LineNumber;
			ColumnNumber = lineInfo.LinePosition + offset;
		}
	}

	public string? File;

	public int LineNumber;

	public int ColumnNumber;

	public int EndLineNumber;

	public int EndColumnNumber;
}
