using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable enable
#pragma warning disable 0067

namespace WPFTest.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public string? OrderInput { get; set; }

		public decimal DecimalProp { get; set; }

		public bool BooleanProp { get; set; }

		public Pag1ModifyViewModel? ModifyViewModel { get; }

		public Task<string> TaskProp => Task.FromResult(string.Empty);

		public IList<EntityViewModel> ListProp { get; } = new List<EntityViewModel>
		{
			new EntityViewModel() { DecimalProp = 1, BooleanProp = true },
		};

		public int[] ArrayProp { get; set; } = new int[0];

		public event PropertyChangedEventHandler? PropertyChanged;

		public async Task<ImageSource> LoadImageAsync()
		{
			await Task.Delay(1000);

			var client = new HttpClient();
			client.DefaultRequestHeaders.UserAgent.ParseAdd("levitali/1.0 (levitali@yahoo.com) bot");

			var img = new BitmapImage();
			img.CacheOption = BitmapCacheOption.OnLoad;
			img.BeginInit();
			img.StreamSource = await client.GetStreamAsync("https://upload.wikimedia.org/wikipedia/commons/thumb/4/43/Bonnet_macaque_%28Macaca_radiata%29_Photograph_By_Shantanu_Kuveskar.jpg/330px-Bonnet_macaque_%28Macaca_radiata%29_Photograph_By_Shantanu_Kuveskar.jpg");
			img.EndInit();
			return img;
		}
	}

	public class Pag1ModifyViewModel
	{
		public string? Input1 { get; set; }
		public string? Input2 { get; set; }
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
