using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PreformanceCompareApp.ViewModels
{

	public class StatisticsViewModel : INotifyPropertyChanged
	{
		private double _Avarage;
		private int _index;
		private readonly long[] _ticks = new long[1000];
		private bool _useAllTicks = false;

		public StatisticsViewModel(IList<TransferOrder> transferOrders)
		{
			TransferOrders = transferOrders.Select(t => new TransferOrderViewModel(this, t)).ToList();
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public IList<TransferOrderViewModel> TransferOrders { get; }

		public double Avarage
		{
			get => _Avarage;
			set
			{
				_Avarage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Avarage)));
			}
		}

		public void UpdateStatistics(long ticks)
		{
			_ticks[_index] = ticks;
			_index++;
			if (_index >= _ticks.Length)
			{
				_index = 0;
				_useAllTicks = true;
			}

			IEnumerable<long> t = _ticks;
			if (!_useAllTicks)
			{
				t = t.Take(_index);
			}

			Avarage = t.Average();
		}

	}
}