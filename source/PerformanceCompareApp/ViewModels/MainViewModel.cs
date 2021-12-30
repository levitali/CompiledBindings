using System;
using System.Collections.Generic;

namespace PreformanceCompareApp.ViewModels
{
	public class MainViewModel
	{
		public MainViewModel()
		{
			var random = new Random();

			string? material = null, materialText = null, quantityUnit = null;
			var list = new List<TransferOrder>(1000);
			for (int i = 0; i < 1000; i++)
			{
				if (i % 10 == 0)
				{
					material = GenerateString('0', 6, false);
					materialText = GenerateString('A', 40, true);
					quantityUnit = random.Next(2) == 0 ? "KG" : "M";
				}

				var item = new TransferOrder
				{
					From = $"{GenerateString('0', 2, false)}-{GenerateString('0', 2, false)}-{GenerateString('0', 2, false)}",
					To = $"{GenerateString('0', 2, false)}-{GenerateString('0', 2, false)}-{GenerateString('0', 2, false)}",
					Material = material!,
					MaterialText = materialText!,
					Quantity = random.Next(1000),
					QuantityUnit = quantityUnit!
				};

				list.Add(item);
			}

			TransferOrders = list;

			string GenerateString(char baseChar, int charCount, bool variableCount)
			{
				string? s = null;
				for (int j = 0, count = variableCount ? random.Next(charCount) : charCount; j < count; j++)
				{
					int ch = baseChar + random.Next(10);
					s += (char)ch;
				}
				return s!;
			}
		}

		public IList<TransferOrder> TransferOrders { get; }
	}
}