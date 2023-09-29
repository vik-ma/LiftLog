using LocalLiftLog.ViewModels;

namespace LocalLiftLog.Pages;

public partial class RoutineDetailsPage : ContentPage
{
	public RoutineDetailsPage(RoutineDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}