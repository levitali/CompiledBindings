using System.Windows.Controls;

namespace WPFTest.Views;

internal partial class Page2 : Page, INotifyPropertyChanged
{
	~Page2()
	{
	}

	public Class2? ObjProp => null;

	public int IntProp => 0;

	public Task<string?> Prop2 => Task.FromResult<string?>("Hello");

	public event PropertyChangedEventHandler? PropertyChanged;

	public void OnClick1(object sender, EventArgs e)
	{
	}

	public void OnClick2()
	{
	}

	public void Confirm(bool isFinal)
	{
	}
}

public class Class2 : INotifyPropertyChanged
{
	public string? Prop1 => "Hello";

	public Task<string?> Prop2 => Task.FromResult<string?>("Hello");

	public event PropertyChangedEventHandler? PropertyChanged;
}