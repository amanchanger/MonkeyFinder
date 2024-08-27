using MonkeyFinder.ViewModel;

namespace MonkeyFinder;

public partial class DetailsPage : ContentPage
{
	public DetailsPage( MonkeyDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}