using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

#nullable enable

namespace XFTest.ViewModels
{
	public class Page1ViewModel : INotifyPropertyChanged
	{
		public decimal DecimalProp { get; set; }

		public bool BooleanProp { get; set; }

		public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
		{
			new EntityViewModel() { DecimalProp = 1, BooleanProp = true },
		};

		public int[] ArrayProp { get; set; } = new[] { 1, 2, 3 };

		public Func<Type, char, bool>? FuncProp { get; set; }	

		public event PropertyChangedEventHandler? PropertyChanged;
	}

	public class EntityViewModel : INotifyPropertyChanged
	{
		public decimal DecimalProp { get; set; }

		public bool BooleanProp { get; set; }

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
