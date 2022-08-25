using System.Windows.Media;

namespace WPFTest.ViewModels;

#nullable enable
#pragma warning disable 0067
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class Page1ViewModel : INotifyPropertyChanged
{
	public string? OrderInput { get; set; }

	public bool BoolInput { get; set; }

	public int IntProp { get; set; }

	[ReadOnly(false)]
	public decimal DecimalProp { get; }

	public bool BooleanProp { get; set; }

	public Page1ModifyViewModel? ModifyViewModel { get; set; }

	public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
	{
		new EntityViewModel { DecimalProp = 1, BooleanProp = true },
	};

	public int[] ArrayProp { get; set; } = new int[0];

	public event PropertyChangedEventHandler? PropertyChanged;

	public Task<string> TaskProp => Task.FromResult(string.Empty);

	public Task<ImageSource> LoadImageAsync()
	{
		return null!;
	}

	public string Calculate(ParamClass prm)
	{
		return "";
	}
}

public class Page1ModifyViewModel : INotifyPropertyChanged
{
	public string? Input1 { get; set; }

	public bool BoolInput { get; set; }

	public int IntInput { get; set; }

	[ReadOnly(false)]
	public Page1ModifyTextViewModel ModifyTextViewModel { get; } = new Page1ModifyTextViewModel();

	public void OnClick(bool parameter)
	{
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class Page1ModifyTextViewModel : INotifyPropertyChanged
{
	public string? TextInput { get; set; }
	public bool? BoolInput { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class EntityViewModel : INotifyPropertyChanged
{
	public string Title { get; set; }

	public EntityModel Model { get; } = new EntityModel();

	public decimal DecimalProp { get; set; }

	public bool BooleanProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class EntityModel : INotifyPropertyChanged
{
	public sbyte SByteProp { get; set; }

	public IList<EntityViewModel>? Children { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class ParamClass
{
	public ParamClass(string str, int i)
	{
	}

	public ParamClass() : this("", 0)
	{
	}

	public ParamClass(string str) : this(str, 0)
	{
	}
}
