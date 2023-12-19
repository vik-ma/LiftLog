namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class SelectWorkoutPage : ContentPage
{
	private readonly SelectWorkoutViewModel _viewModel;
	public SelectWorkoutPage(SelectWorkoutViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
    }
}