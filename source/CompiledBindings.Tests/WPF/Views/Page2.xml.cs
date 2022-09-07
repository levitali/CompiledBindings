using System.Windows.Controls;



namespace WPFTest.Views;

internal partial class Page2 : Page
{
	~Page2()
	{
	}

	public string Prop1 => "Hello";

	public Task<string> Prop2 => Task.FromResult("Hello");

	public void OnClick1(object sender, EventArgs e)
	{
	}

	public void OnClick2()
	{
	}
}

