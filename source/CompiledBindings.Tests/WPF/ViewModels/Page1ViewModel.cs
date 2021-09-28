using System.Collections.Generic;
using System.ComponentModel;

namespace WPFTest.ViewModels
{
	public class Page1ViewModel : INotifyPropertyChanged
	{
		public string OrderInput { get; set; }

		public bool BoolInput { get; set; }

		public decimal DecimalProp { get; }

		public bool BooleanProp { get; }

		public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
		{
			new EntityViewModel() { DecimalProp = 1, BooleanProp = true },
		};

		public int[] ArrayProp { get; }

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class EntityViewModel : INotifyPropertyChanged
	{
		public EntityModel Model { get; }


		public decimal DecimalProp { get; set; }

		public bool BooleanProp { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class EntityModel : INotifyPropertyChanged
	{
		public sbyte SByteProp { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
