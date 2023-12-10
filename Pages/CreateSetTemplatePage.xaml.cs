namespace LocalLiftLog.Pages;

using LocalLiftLog.Models;
using LocalLiftLog.ViewModels;

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
}