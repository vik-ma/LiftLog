namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class ExerciseListPage : ContentPage
{
    private readonly ExerciseListViewModel _viewModel;
    public ExerciseListPage(ExerciseListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadExerciseGroupIntList();
        await _viewModel.LoadExercisesAsync();
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedExercise = (int)args.SelectedItem;
            _viewModel.AddExerciseGroupToFilterList(selectedExercise);
        }
    }
}