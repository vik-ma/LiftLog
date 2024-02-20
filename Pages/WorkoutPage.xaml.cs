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

        // Prevent reloading old page if navigated
        // from WorkoutOperatingPage or ExerciseDetailsPage
        if (!_viewModel.IsWorkoutLoaded) 
        {
            _viewModel.LoadSelectedDateTime();
            await _viewModel.LoadSetList();
        }
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        DateTime selectedDate = e.NewDate;

        await _viewModel.UpdateWorkoutDate(selectedDate);
    }

    private void DropGestureRecognizer_DragOver(object sender, DragEventArgs e)
    {
        e.PlatformArgs.DragEventArgs.DragUIOverride.IsGlyphVisible = false;
        e.PlatformArgs.DragEventArgs.DragUIOverride.IsCaptionVisible = false;
    }

    private void DropGestureRecognizer_DragLeave(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.None;
    }
}