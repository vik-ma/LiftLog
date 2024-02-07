namespace LocalLiftLog.Pages;

public partial class WorkoutTemplateListPopupPage : Popup
{
    private readonly string _viewModelType;

    private readonly WorkoutViewModel _workoutViewModel;
    private readonly ScheduleViewModel _scheduleViewModel;
    private readonly WorkoutDetailsViewModel _workoutDetailsViewModel;

    #nullable enable
    public WorkoutTemplateListPopupPage(string viewModelType, WorkoutViewModel? workoutViewModel, ScheduleViewModel? scheduleViewModel, WorkoutDetailsViewModel? workoutDetailsViewModel)
	{
		InitializeComponent();
        _viewModelType = viewModelType;

        switch (viewModelType) {
            case "Workout":
                _workoutViewModel = workoutViewModel;
                BindingContext = _workoutViewModel;
                break;
            case "Schedule":
                _scheduleViewModel = scheduleViewModel;
                BindingContext = _scheduleViewModel;
                break;
            case "WorkoutDetails":
                _workoutDetailsViewModel = workoutDetailsViewModel;
                BindingContext = _workoutDetailsViewModel;
                break;
            default:
                return;
        }
    }

    private void OnWorkoutTemplateItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedWorkoutTemplate = (WorkoutTemplate)args.SelectedItem;

            if (_viewModelType == "Workout" && _workoutViewModel is not null) 
            {
                OnItemSelectedForWorkoutViewModel(selectedWorkoutTemplate);
            }
            if (_viewModelType == "Schedule" && _scheduleViewModel is not null)
            {
                OnItemSelectedForScheduleViewModel(selectedWorkoutTemplate);
            }
            if (_viewModelType == "WorkoutDetails" && _workoutDetailsViewModel is not null)
            {
                OnItemSelectedForWorkoutDetailsViewModel(selectedWorkoutTemplate);
            }
        }
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs args)
    {
        string filterText = args.NewTextValue.ToLowerInvariant();

        if (_viewModelType == "Workout" && _workoutViewModel is not null)
        {
            OnFilterTextChangedForWorkoutViewModel(filterText);
        }
        if (_viewModelType == "Schedule" && _scheduleViewModel is not null)
        {
            OnFilterTextChangedForScheduleViewModel(filterText);
        }
        if (_viewModelType == "WorkoutDetails" && _workoutDetailsViewModel is not null)
        {
            OnFilterTextChangedForWorkoutDetailsViewModel(filterText);
        }
    }

    private async void OnItemSelectedForWorkoutViewModel(WorkoutTemplate selectedWorkoutTemplate)
    {
        if (selectedWorkoutTemplate is null) return;

        await _workoutViewModel.UpdateOperatingWorkoutTemplate(selectedWorkoutTemplate.Id);
        _workoutViewModel.ClosePopup();
    }

    private void OnFilterTextChangedForWorkoutViewModel(string filterText)
    {
        _workoutViewModel.FilteredWorkoutTemplateList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
        {
            _workoutViewModel.FilteredWorkoutTemplateList = new(_workoutViewModel.WorkoutTemplateList);
        }
        else
        {
            // Filter the list based on the user input
            _workoutViewModel.FilteredWorkoutTemplateList = new(_workoutViewModel.WorkoutTemplateList.Where(item => item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
        }
    }

    private async void OnItemSelectedForScheduleViewModel(WorkoutTemplate selectedWorkoutTemplate)
    {
        if (selectedWorkoutTemplate is null) return;

        await _scheduleViewModel.AddWorkoutTemplateCollectionToDay(selectedWorkoutTemplate);
        _scheduleViewModel.ClosePopup();
    }

    private void OnFilterTextChangedForScheduleViewModel(string filterText)
    {
        _scheduleViewModel.FilteredWorkoutTemplateList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
        {
            _scheduleViewModel.FilteredWorkoutTemplateList = new(_scheduleViewModel.WorkoutTemplateList);
        }
        else
        {
            // Filter the list based on the user input
            _scheduleViewModel.FilteredWorkoutTemplateList = new(_scheduleViewModel.WorkoutTemplateList.Where(item => item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
        }
    }

    private async void OnItemSelectedForWorkoutDetailsViewModel(WorkoutTemplate selectedWorkoutTemplate)
    {
        if (selectedWorkoutTemplate is null) return;

        await _workoutDetailsViewModel.CopyWorkout(selectedWorkoutTemplate.Id);
        _workoutDetailsViewModel.ClosePopup();
    }

    private void OnFilterTextChangedForWorkoutDetailsViewModel(string filterText)
    {
        _workoutDetailsViewModel.FilteredWorkoutTemplateList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
        {
            _workoutDetailsViewModel.FilteredWorkoutTemplateList = new(_workoutDetailsViewModel.WorkoutTemplateList);
        }
        else
        {
            // Filter the list based on the user input
            _workoutDetailsViewModel.FilteredWorkoutTemplateList = new(_workoutDetailsViewModel.WorkoutTemplateList.Where(item => item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}