namespace LocalLiftLog.Pages;

public partial class WorkoutRoutineListPopupPage : Popup
{
	private readonly WorkoutRoutineDetailsViewModel _viewModel;
	public WorkoutRoutineListPopupPage(WorkoutRoutineDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnWorkoutRoutineItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedWorkoutRoutine = (WorkoutRoutine)args.SelectedItem;
            await _viewModel.CopyWorkoutRoutineSchedule(selectedWorkoutRoutine);
            _viewModel.ClosePopup();
        }
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs args)
    {
        string filterText = args.NewTextValue.ToLowerInvariant();

        _viewModel.FilteredWorkoutRoutineList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
            _viewModel.FilteredWorkoutRoutineList = new(_viewModel.WorkoutRoutineList);
        else
            _viewModel.FilteredWorkoutRoutineList = new(_viewModel.WorkoutRoutineList.Where(item => item.Name != null && item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
    }
}