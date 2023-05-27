using System.Collections.Generic;
using System.ComponentModel;

namespace UWPTest.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public string ButtonText => "Navigate to Page1";

	public int Prop1 { get; set; } = 3;

	public Class1 Class1 { get; } = new Class1();

	public string? StringInput { get; set; }

	public double InputProp1 { get; set; }

	public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
	{
		new EntityViewModel() { IntProp = 100 },
	};
}

public class EntityViewModel
{
	public int IntProp { get; set; }
	public string? StringProp { get; set; }
}

public class Class1 : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public string Prop2 { get; set; } = "test";
}