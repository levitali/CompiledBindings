namespace WPFTest;



public sealed partial class Strings
{
	public static Strings Instance { get; } = new Strings();

	public string? Category { get; set; }

	public string Title => "";

	public string Header1 => "";
}

