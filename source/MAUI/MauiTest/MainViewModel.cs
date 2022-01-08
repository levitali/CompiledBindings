using System.ComponentModel;

#nullable enable

namespace MauiTest;

public class MainViewModel : INotifyPropertyChanged
{
	private string? _Input1;

	public string Hello => "Hello, CompiledBindings!";
	public static string Welcome => "Welcome to .NET Multi-platform App UI with CompiledBindings";

	public string? Input1
	{
		get => _Input1;
		set
		{
			_Input1 = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Input1)));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

