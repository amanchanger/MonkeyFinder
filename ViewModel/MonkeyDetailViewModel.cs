using CommunityToolkit.Mvvm.ComponentModel;
using MonkeyFinder.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MonkeyFinder.ViewModel;
    [QueryProperty("Monkey","Monkey")]
public partial class MonkeyDetailViewModel: ObservableObject
    {
    IMap map;
    public Command OpenMapCommand { get; }
        public MonkeyDetailViewModel( IMap map)
    {
        this.map = map;
        OpenMapCommand = new Command(async () => await OpenMapAsync());
    }


    [ObservableProperty]
        Monkey monkey;

    async Task OpenMapAsync()
    {
        try
        {
            await map.OpenAsync(Monkey.Latitude, Monkey.Longitude,
                new MapLaunchOptions
                {
                    Name = Monkey.Name,
                    NavigationMode = NavigationMode.None
                });
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!!",
                $"Unable To Open Map:{ex.Message}", "Ok");
        }
    }
}
