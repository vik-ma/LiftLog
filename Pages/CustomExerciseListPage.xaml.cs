namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class CustomExerciseListPage : ContentPage
{
	private readonly CustomExerciseViewModel _viewModel;
	public CustomExerciseListPage(CustomExerciseViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCustomExercisesAsync();
    }
}