namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class WorkoutRoutineListPage : ContentPage
{
    private readonly WorkoutRoutineListViewModel _viewModel;

    public WorkoutRoutineListPage(WorkoutRoutineListViewModel viewModel)
    {
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWorkoutRoutinesAsync();
    }
}