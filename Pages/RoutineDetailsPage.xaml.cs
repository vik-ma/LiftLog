namespace LocalLiftLog.Pages; 
using LocalLiftLog.ViewModels;

public partial class RoutineDetailsPage : ContentPage
{
    private readonly RoutineDetailsViewModel _viewModel;
    public RoutineDetailsPage(RoutineDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSchedulesAsync();
    }
}