using System.Diagnostics;

namespace PreformanceCompareApp.ViewModels
{
	public class TransferOrder
	{
		public string From { get; init; } = null!;

		public string To { get; init; } = null!;

		public decimal Quantity { get; init; }

		public string QuantityUnit { get; init; } = null!;

		public string Material { get; init; } = null!;

		public string MaterialText { get; init; } = null!;
	}

	public class TransferOrderViewModel
	{
		private readonly StatisticsViewModel _parent;
		private readonly TransferOrder _model;
		private int _fieldCounter;
		private readonly Stopwatch _stopwatch = new Stopwatch();

		public TransferOrderViewModel(StatisticsViewModel parent, TransferOrder model)
		{
			_parent = parent;
			_model = model;
		}

		public string From => GetProperty(_model.From);

		public string To => GetProperty(_model.To);

		public decimal Quantity => GetProperty(_model.Quantity);

		public string QuantityUnit => GetProperty(_model.QuantityUnit);

		public string Material => GetProperty(_model.Material);

		public string MaterialText => GetProperty(_model.MaterialText);

		private T GetProperty<T>(T value)
		{
			if (_fieldCounter == 0)
			{
				_stopwatch.Start();
			}
			_fieldCounter++;
			if (_fieldCounter == 6)
			{
				_stopwatch.Stop();
				_fieldCounter = 0;

				_parent.UpdateStatistics(_stopwatch.ElapsedTicks);
				_stopwatch.Reset();
			}
			return value;
		}
	}
}