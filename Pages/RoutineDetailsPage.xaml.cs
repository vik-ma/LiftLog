namespace LocalLiftLog.Pages; 
using LocalLiftLog.ViewModels;

public partial class RoutineDetailsPage : ContentPage
{
	public RoutineDetailsPage(RoutineDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}