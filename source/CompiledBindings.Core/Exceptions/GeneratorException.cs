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

	public GeneratorException(string message, string file, XObject xnode, int offset = 0, int length = 0) : base(message)
	{
		File = file;
		if (xnode is IXmlLineInfo lineInfo)
		{
			LineNumber = EndLineNumber = lineInfo.LineNumber;
			ColumnNumber = lineInfo.LinePosition;
		}
		ColumnNumber += offset;
		EndColumnNumber = ColumnNumber + length; 
	}

	public GeneratorException(string message, string file, XamlNode xamlNode, int offset = 0, int length = 0) 
		: this(message, file, xamlNode.Element, offset, length)
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

	public int LineNumber { get; }

	public int ColumnNumber { get; }

	public int EndLineNumber { get; }

	public int EndColumnNumber { get; }
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
