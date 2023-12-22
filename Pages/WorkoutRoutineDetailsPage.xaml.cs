namespace LocalLiftLog.Pages; 
using LocalLiftLog.ViewModels;

public partial class WorkoutRoutineDetailsPage : ContentPage
{
    private readonly WorkoutRoutineDetailsViewModel _viewModel;
    public WorkoutRoutineDetailsPage(WorkoutRoutineDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWorkoutRoutineSchedule();
    }
}