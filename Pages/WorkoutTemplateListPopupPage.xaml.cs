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
}