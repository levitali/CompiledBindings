using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable enable

namespace XFTest.ViewModels;

public class Page2ViewModel : INotifyPropertyChanged
{
	public decimal StringProp { get; set; }

	public decimal DecimalProp { get; set; }

	public DateTime DateTimeProp { get; set; }

	public ItemViewModel? CurrentItem { get; set; }

	public GroupViewModel? Group { get; set; }

	public string CalculateString() => "";

	public (string fontFamily, string? glyph) GetIcon() => ("", "");

	public Func<string, ItemViewModel>? FuncProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class ItemViewModel : INotifyPropertyChanged
{
	public Guid GuidProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class GroupViewModel : List<ItemViewModel>
{
}

