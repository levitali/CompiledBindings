using System.Collections.Generic;
using System.ComponentModel;

#nullable enable
#pragma warning disable 0067

namespace WPFTest.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public string? OrderInput { get; set; }

		public decimal DecimalProp { get; set; }

		public bool BooleanProp { get; set; }

		public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
		{
			new EntityViewModel() { DecimalProp = 1, BooleanProp = true },
		};

		public int[] ArrayProp { get; set; } = new int[0];

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
}
