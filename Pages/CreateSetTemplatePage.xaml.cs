namespace LocalLiftLog.Pages;
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
}