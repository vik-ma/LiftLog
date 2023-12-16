namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class WorkoutDetailsPage : ContentPage
{
    private readonly WorkoutDetailsViewModel _viewModel;
    public WorkoutDetailsPage(WorkoutDetailsViewModel viewModel)
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