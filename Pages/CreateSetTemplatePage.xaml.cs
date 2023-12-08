namespace LocalLiftLog.Pages;

using LocalLiftLog.Models;
using LocalLiftLog.ViewModels;

public partial class CreateSetTemplatePage : ContentPage
{
    private readonly WorkoutDetailsViewModel _viewModel;
    public CreateSetTemplatePage(WorkoutDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadExerciseListAsync();
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedExercise = (Exercise)args.SelectedItem;
            _viewModel.NewSetTemplateSelectedExerciseName = selectedExercise.Name;
        }
    }
}