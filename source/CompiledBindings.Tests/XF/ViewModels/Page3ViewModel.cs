namespace XFTest.ViewModels;

#pragma warning disable 0067

public class Page3ViewModel : INotifyPropertyChanged
{
	public bool IsLoading { get; set; }

	public EntityModel? Entity { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

