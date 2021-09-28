#nullable enable

namespace WPFTest
{
	public sealed partial class Strings
	{
		public static Strings Instance { get; } = new Strings();

		public string? Category { get; set; }

		public string? Title => "Test";

		public string? Header1 => "Header1";

	}
}
