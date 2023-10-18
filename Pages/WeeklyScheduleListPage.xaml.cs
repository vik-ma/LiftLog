namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class WeeklyScheduleListPage : ContentPage
{
    private readonly WeeklyScheduleViewModel _viewModel;

    public WeeklyScheduleListPage(WeeklyScheduleViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSchedulesAsync();
    }
}