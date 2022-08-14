#nullable enable
#pragma warning disable 0067

namespace XFTest.ViewModels;

public class Page2ViewModel : INotifyPropertyChanged
{
	public decimal StringProp { get; set; }

	public decimal DecimalProp { get; set; }

	public DateTime DateTimeProp { get; set; }

	public ItemViewModel? CurrentItem { get; set; }

	public Item2ViewModel? CurrentItem2 { get; set; }

	public GroupViewModel? Group { get; set; }

	public string CalculateString()
	{
		return "";
	}

	public (string fontFamily, string? glyph) GetIcon()
	{
		return ("", "");
	}

	public Func<string, ItemViewModel>? FuncProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class ItemViewModel : INotifyPropertyChanged
{
	public Guid GuidProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class Item2ViewModel : INotifyPropertyChanged
{
	public string? Prop1 { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class GroupViewModel : List<ItemViewModel>
{
}

