namespace LocalLiftLog.Pages;

public partial class CreateExercisePopupPage : Popup
{
	private readonly ExerciseListViewModel _viewModel;
	public CreateExercisePopupPage(ExerciseListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            _viewModel.AddExerciseGroupToOperatingExercise(selectedIndex);
        }
    }
}