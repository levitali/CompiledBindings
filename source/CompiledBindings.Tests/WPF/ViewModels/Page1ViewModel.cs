using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFTest.ViewModels;

#nullable enable
#pragma warning disable 0067

public class Page1ViewModel : INotifyPropertyChanged
{
	public string? OrderInput { get; set; }

	public bool BoolInput { get; set; }

	public decimal DecimalProp { get; }

	public bool BooleanProp { get; }

	public Page1ModifyViewModel? ModifyViewModel { get; }

	public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
	{
		new EntityViewModel() { DecimalProp = 1, BooleanProp = true },
	};

	public int[] ArrayProp { get; } = new int[0];

	public event PropertyChangedEventHandler? PropertyChanged;

	public Task<string> TaskProp => Task.FromResult(string.Empty);

	public Task<ImageSource> LoadImageAsync() => null!;

	public string Calculate(ParamClass prm) => "";
}

public class Page1ModifyViewModel : INotifyPropertyChanged
{
	public string? Input1 { get; set; }

	public bool BoolInput { get; set; }

	public Page1ModifyTextViewModel ModifyTextViewModel { get; } = new Page1ModifyTextViewModel();

	public void OnClick(bool parameter)
	{
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class Page1ModifyTextViewModel : INotifyPropertyChanged
{
	public string? TextInput { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class EntityViewModel : INotifyPropertyChanged
{
	public EntityModel Model { get; } = new EntityModel();

	public decimal DecimalProp { get; set; }

	public bool BooleanProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class EntityModel : INotifyPropertyChanged
{
	public sbyte SByteProp { get; set; }

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
