namespace LocalLiftLog.Pages;
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