namespace LocalLiftLog.Pages;
public partial class SchedulePage : ContentPage
{
    private readonly ScheduleViewModel _viewModel;
    public SchedulePage(ScheduleViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWorkoutTemplateCollectionsAsync();
        await _viewModel.LoadWorkoutTemplatesAsync();
    }
}