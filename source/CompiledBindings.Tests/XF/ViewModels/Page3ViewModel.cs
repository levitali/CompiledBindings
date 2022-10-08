namespace XFTest.ViewModels;

#pragma warning disable 0067

public class Page3ViewModel : INotifyPropertyChanged
{
	public bool IsLoading { get; set; }

	public EntityModel? Entity { get; set; }

	public IList<PickItem> PickedItems { get; } = new List<PickItem>();

	public PickItem? SelectedPickItem { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class PickItem : INotifyPropertyChanged
{
	public string Description { get; set; } = string.Empty;

	public int Id { get; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

