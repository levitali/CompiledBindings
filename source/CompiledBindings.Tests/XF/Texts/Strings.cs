namespace XFTest;



public sealed partial class Strings
{
	public static Strings Instance { get; } = new Strings();

	public string? Category { get; set; }

	public string? Title => null;

	public string? Header1 => null;

}

