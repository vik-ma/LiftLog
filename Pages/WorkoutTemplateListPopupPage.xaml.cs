namespace LocalLiftLog.Pages;

public partial class WorkoutTemplateListPopupPage : Popup
{
    private readonly WorkoutViewModel _viewModel;
    public WorkoutTemplateListPopupPage(WorkoutViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnWorkoutTemplateItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedWorkoutTemplate = (WorkoutTemplate)args.SelectedItem;
            await _viewModel.UpdateOperatingWorkoutTemplate(selectedWorkoutTemplate.Id);
            _viewModel.ClosePopup();
        }
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs args)
    {
        string filterText = args.NewTextValue.ToLowerInvariant();

        _viewModel.FilteredWorkoutTemplateList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
        {
            _viewModel.FilteredWorkoutTemplateList = new(_viewModel.WorkoutTemplateList);
        }
        else
        {
            // Filter the list based on the user input
            var filteredItems = _viewModel.WorkoutTemplateList.Where(item => item.Name.ToLowerInvariant().Contains(filterText));

            foreach (var item in filteredItems)
            {
                _viewModel.FilteredWorkoutTemplateList.Add(item);
            }
        }
    }
}