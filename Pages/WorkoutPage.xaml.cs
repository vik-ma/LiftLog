namespace LocalLiftLog.Pages;
public partial class WorkoutPage : ContentPage
{
    private readonly WorkoutViewModel _viewModel;
    public WorkoutPage(WorkoutViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadSelectedDateTime();
        await _viewModel.LoadSetList();
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        DateTime selectedDate = e.NewDate;

        await _viewModel.UpdateWorkoutDate(selectedDate);
    }
}