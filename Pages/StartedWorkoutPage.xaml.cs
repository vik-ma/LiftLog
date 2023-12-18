namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class StartedWorkoutPage : ContentPage
{
    private readonly StartedWorkoutViewModel _viewModel;
    public StartedWorkoutPage(StartedWorkoutViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSetListFromWorkoutTemplateIdAsync();
    }
}