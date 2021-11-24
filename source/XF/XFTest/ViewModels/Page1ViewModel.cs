using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

#nullable enable

namespace XFTest.ViewModels
{
	public class Page1ViewModel : INotifyPropertyChanged
	{
		public ModifyPage1ViewModel ModifyViewModel { get; } = new ();

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

	public class ModifyPage1ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public string? Input1 { get; set; }	
		public int? Input2 { get; set; }
	}

	public class EntityViewModel : INotifyPropertyChanged
	{
		public decimal DecimalProp { get; set; }

		public bool BooleanProp { get; set; }

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
