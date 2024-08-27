using MonkeyFinder.ViewModel;

namespace MonkeyFinder;
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        // Name of (DetailsPage)== "Detailspage"
        Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
        }
    }
