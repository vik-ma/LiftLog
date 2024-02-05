namespace LocalLiftLog.Pages;

public partial class WorkoutTemplateListPopupPage : Popup
{
    private readonly string _viewModelType;

    private readonly WorkoutViewModel _workoutViewModel;
    private readonly ScheduleViewModel _scheduleViewModel;

    #nullable enable
    public WorkoutTemplateListPopupPage(string viewModelType, WorkoutViewModel? workoutViewModel, ScheduleViewModel? scheduleViewModel)
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
            var filteredItems = _workoutViewModel.WorkoutTemplateList.Where(item => item.Name.ToLowerInvariant().Contains(filterText));

            foreach (var item in filteredItems)
            {
                _workoutViewModel.FilteredWorkoutTemplateList.Add(item);
            }
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
            var filteredItems = _scheduleViewModel.WorkoutTemplateList.Where(item => item.Name.ToLowerInvariant().Contains(filterText));

            foreach (var item in filteredItems)
            {
                _scheduleViewModel.FilteredWorkoutTemplateList.Add(item);
            }
        }
    }
}