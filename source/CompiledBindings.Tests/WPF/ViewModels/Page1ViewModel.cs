using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFTest.ViewModels;

#nullable enable

public class Page1ViewModel : INotifyPropertyChanged
{
	public string? OrderInput { get; set; }

	public bool BoolInput { get; set; }

	public decimal DecimalProp { get; }

	public bool BooleanProp { get; }

	public Pag1ModifyViewModel? ModifyViewModel { get; }

	public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
	{
		new EntityViewModel() { DecimalProp = 1, BooleanProp = true },
	};

	public int[] ArrayProp { get; }

	public event PropertyChangedEventHandler? PropertyChanged;

	public Task<string> TaskProp => Task.FromResult(string.Empty);

	public Task<ImageSource> LoadImageAsync() => null!;
}

public class Pag1ModifyViewModel : INotifyPropertyChanged
{
	public string? Input1 { get; set; }

	public void OnClick(bool parameter)
	{
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class EntityViewModel : INotifyPropertyChanged
{
	public EntityModel Model { get; }

	public decimal DecimalProp { get; set; }

	public bool BooleanProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class EntityModel : INotifyPropertyChanged
{
	public sbyte SByteProp { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}
