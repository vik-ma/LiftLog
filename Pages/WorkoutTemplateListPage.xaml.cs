namespace LocalLiftLog.Pages;
public partial class WorkoutTemplateListPage : ContentPage
{
    private readonly WorkoutTemplateViewModel _viewModel;
    public WorkoutTemplateListPage(WorkoutTemplateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWorkoutTemplatesAsync();
        await _viewModel.LoadWorkoutTemplateCollectionsAsync();
    }
}