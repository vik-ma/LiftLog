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
        string selectedItem = (string)picker.SelectedItem;

        if (!string.IsNullOrEmpty(selectedItem))
        {
            if (!ExerciseGroupDictionary.FlippedExerciseGroupDict.TryGetValue(selectedItem, out int exerciseGroupInt))
                return;

            _viewModel.AddExerciseGroupToOperatingExercise(exerciseGroupInt);
        }
    }
}