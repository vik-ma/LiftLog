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
        await _viewModel.LoadExercisesAsync();
    }
}