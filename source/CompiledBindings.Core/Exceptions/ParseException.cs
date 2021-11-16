namespace CompiledBindings;

public sealed class ParseException : Exception
{
	public ParseException(string message, int position = 0, int length = 0)
		: base(message)
	{
		Position = position;
		Length = length;
	}

	public int Position { get; }

	public int Length { get; }
}

