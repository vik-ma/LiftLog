namespace LocalLiftLog.Pages;

public partial class ExerciseDetailsPage : ContentPage
{
	private readonly ExerciseDetailsViewModel _viewModel;
	public ExerciseDetailsPage(ExerciseDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSetsFromExerciseIdAsync();
    }
}