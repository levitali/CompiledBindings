using System.Runtime.CompilerServices;

namespace XFTest.ViewModels;

#pragma warning disable 0067

public class Page3ViewModel : INotifyPropertyChanged
{
	public bool IsLoading { get; set; }

	public decimal? QuantityInput { get; set; }

	public int PortInput { get; set; }

	public EntityModel? Entity { get; set; }

	public IList<PickItem> PickedItems { get; } = new List<PickItem>();

	public PickItem? SelectedPickItem { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;

	[IndexerName("State")]
	public string? this[string index]
	{
		get => null;
		set { }
	}

	[IndexerName("State")]
	public int this[int index, string name]
	{
		get => 0;
	}
}

public class PickItem : INotifyPropertyChanged
{
	public string Description { get; set; } = string.Empty;

	public int Id { get; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

