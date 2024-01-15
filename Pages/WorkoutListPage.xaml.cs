namespace LocalLiftLog.Pages;
public partial class WorkoutListPage : ContentPage
{
    private readonly WorkoutListViewModel _viewModel;
    public WorkoutListPage(WorkoutListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWorkoutsAsync();
    }
}