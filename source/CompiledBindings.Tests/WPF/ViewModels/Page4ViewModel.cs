namespace WPFTest.ViewModels;

public class Page4ViewModel : INotifyPropertyChanged
{
	public SubViewModel SubViewModel { get; set; } = new();
	
	public SubViewModel SubViewModel2 { get; set; } = new();
	
	public bool IsEnabled { get; set; }
	
	public event PropertyChangedEventHandler? PropertyChanged;
}

public class SubViewModel : INotifyPropertyChanged
{
	public SubSubViewModel SubSubViewModel { get; set; } = new();
	
	public bool IsEditingSub { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class SubSubViewModel : INotifyPropertyChanged
{
	public bool IsEditingSubSub { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

