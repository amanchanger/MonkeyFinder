using MonkeyFinder.Service;
using MonkeyFinder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
    MonkeyService monkeyService;
    public ObservableCollection<Monkey> Monkeys { get; } = new();

    IConnectivity connectivity;
    IGeolocation geolocation;
    public Command GetMonkeyCommand { get; }
    public Command GetDetailsCommand { get; }
    public Command GetClosestMonkeyCommand { get; }

    bool isRefreshing;
    public MonkeysViewModel(MonkeyService monkeyService , IConnectivity connectivity, IGeolocation geolocation)
    {
        Title = "Monkey Finder";
        this.monkeyService = monkeyService;
        this.connectivity = connectivity;
        GetMonkeyCommand = new Command(async () => await GetMonkeyAsync());
        GetClosestMonkeyCommand = new Command(async () => await GetClosestMonkeyAsync());
        GetDetailsCommand = new Command<Monkey>(async (monkey) => await GoToDetailsAsync(monkey));
        this.geolocation = geolocation;
    }
    
    async Task GetClosestMonkeyAsync()
    {
        if (IsBusy || Monkeys.Count == 0)
            return;
        try
        {
            var location = await geolocation.GetLastKnownLocationAsync();
            if (location is null)
            {
                location = await geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy= GeolocationAccuracy.Medium,
                        Timeout= TimeSpan.FromSeconds(30),
                    });
            }
            if (location is null)
                return;
            var first = Monkeys.OrderBy( m =>
               location.CalculateDistance(m.Latitude,m.Longitude,DistanceUnits.Miles)
               ).FirstOrDefault();

            if(first is null)
                return;

            await Shell.Current.DisplayAlert("Closest Monkey", $"{first.Name}, in {first.Location}", "Ok");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!!",
                $"Unable To Get Closest Monkey :{ex.Message}", "Ok");
        }
    }

    async Task GoToDetailsAsync(Monkey monkey)
    {
        if (monkey is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}",true,
            new Dictionary<string,object>
            {
                {"Monkey", monkey }
            });
    }
   
    public bool IsRefreshing
    {
        get => isRefreshing;
        set
        {
            isRefreshing = value;
            OnPropertyChanged(nameof(IsRefreshing));
        }
    }


    private object DetailsPage()
    {
        throw new NotImplementedException();
    }

    async Task GetMonkeyAsync()
    {       
        if (IsBusy)
            return;
        try
        {
            if(Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet Issue!!", $"Check Ur Internet And Try Again!!", "Ok");
                return;
            }

            IsBusy = true;
            var monkeys = await monkeyService.GetMonkeys();
            if (Monkeys.Count != 0)
                Monkeys.Clear();
            foreach (var monkey in monkeys)
                Monkeys.Add(monkey);
           
        }
        catch (Exception ex)
        {
         Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!!", $"Unable To Get Monkeys :{ex.Message}","Ok");
        }
        finally
        {
            IsBusy=false;
            IsRefreshing = false;
        }
    }
}
