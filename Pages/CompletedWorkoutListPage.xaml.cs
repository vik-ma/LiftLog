namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class CompletedWorkoutListPage : ContentPage
{
    private readonly CompletedWorkoutViewModel _viewModel;
	public CompletedWorkoutListPage(CompletedWorkoutViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        //await _viewModel.LoadCompletedWorkoutsAsync();
    }
}