namespace LocalLiftLog.Pages;

using LocalLiftLog.Models;
using LocalLiftLog.ViewModels;
using System.Collections.ObjectModel;

public partial class CreateSetTemplatePage : ContentPage
{
    private readonly CreateSetTemplateViewModel _viewModel;
    public CreateSetTemplatePage(CreateSetTemplateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadExerciseListAsync();
        _viewModel.InitializeSetWorkoutTemplatePackage();
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedExercise = (Exercise)args.SelectedItem;
            _viewModel.NewSetTemplateSelectedExerciseName = selectedExercise.Name;
        }
    }

    void OnFilterTextChanged(object sender, TextChangedEventArgs args) 
    {
        string filterText = args.NewTextValue.ToLowerInvariant();

        _viewModel.FilteredExerciseList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
        {
            _viewModel.FilteredExerciseList = new(_viewModel.ExerciseList);
        }
        else
        {
            // Filter the list based on the user input
            var filteredItems = _viewModel.ExerciseList.Where(item => item.Name.ToLowerInvariant().Contains(filterText));
            
            foreach (var item in filteredItems)
            {
                _viewModel.FilteredExerciseList.Add(item);
            }
        }
    }
}