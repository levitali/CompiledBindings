using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace MauiTest;

public class MainViewModel : INotifyPropertyChanged
{
	public string Hello => "Hello, CompiledBindings!";

	public static string Welcome => "Welcome to .NET Multi-platform App UI with CompiledBindings";

	public event PropertyChangedEventHandler? PropertyChanged;
}

