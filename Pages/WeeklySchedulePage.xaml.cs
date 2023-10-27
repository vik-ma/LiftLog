namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class WeeklySchedulePage : ContentPage
{
    private readonly WeeklyScheduleViewModel _viewModel;

    public WeeklySchedulePage(WeeklyScheduleViewModel viewModel)
	{
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWorkoutTemplateCollectionsAsync();
    }
}