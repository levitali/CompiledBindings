using System.Collections.Generic;
using System.ComponentModel;
using UI;

#nullable enable
#pragma warning disable 0067

namespace XFTest.ViewModels;

public class Page1ViewModel : INotifyPropertyChanged
{
	public decimal DecimalProp { get; set; }

	public bool BooleanProp { get; set; }

	public int? NullableIntProp { get; set; }

	public TestEnum EnumProp { get; set; }

	public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
	{
		new EntityViewModel() { DecimalProp = 1, BooleanProp = true },
	};

	public int[]? ArrayProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetValue(string value)
	{
	}

	public FocusState<Field> FocusedField { get; private set; } = new FocusState<Field>();

	public string? StringProp { get; set; }

	public enum Field
	{
		Field1,
		Field2
	}
}

public class EntityViewModel : INotifyPropertyChanged
{
	public EntityModel? Model { get; }

	public string? StringProp { get; set; }

	public decimal DecimalProp { get; set; }

	public bool BooleanProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class EntityModel : INotifyPropertyChanged
{
	public sbyte SByteProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public enum TestEnum
{
	Value1,
	Value2
}

