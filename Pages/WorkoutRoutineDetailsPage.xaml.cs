namespace LocalLiftLog.Pages;
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
        await _viewModel.LoadScheduleAsync();
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        DateTime selectedDate = e.NewDate;

        await _viewModel.SetCustomScheduleStartDate(selectedDate);
    }
}