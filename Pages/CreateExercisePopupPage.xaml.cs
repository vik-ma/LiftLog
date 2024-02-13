using Microsoft.VisualBasic;
using System.Collections.Immutable;

namespace LocalLiftLog.Pages;

public partial class CreateExercisePopupPage : Popup
{
	private readonly ExerciseListViewModel _viewModel;
	public CreateExercisePopupPage(ExerciseListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        PopulatePickerItems();
    }

    private void PopulatePickerItems()
    {
        foreach (var item in _viewModel.PickerExerciseGroupIntList)
        {
            if (ExerciseGroupDictionary.ExerciseGroupDict.TryGetValue(item, out string exerciseGroupString))
                ExerciseGroupPicker.Items.Add(exerciseGroupString);
        }
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

            ExerciseGroupPicker.Items.Remove(selectedItem);
            ExerciseGroupPicker.SelectedItem = null;
        }
    }

    private void RemoveButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        int exerciseGroupInt = (int)button.BindingContext;

        _viewModel.RemoveExerciseGroupFromOperatingExercise(exerciseGroupInt);

        if (ExerciseGroupDictionary.ExerciseGroupDict.TryGetValue(exerciseGroupInt, out string exerciseGroupString))
        {
            string[] itemsArray = [.. ExerciseGroupPicker.Items];

            // Find the index where exerciseGroupString should be inserted
            int index = Array.BinarySearch(itemsArray, exerciseGroupString);

            if (index < 0)
            {
                // Convert the negative index returned by BinarySearch to the index where exerciseGroupString should be inserted
                index = ~index;
            }

            // Insert exerciseGroupString where it should be alphabetically
            ExerciseGroupPicker.Items.Insert(index, exerciseGroupString);
        }  
    }
}