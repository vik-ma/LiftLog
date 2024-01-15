namespace LocalLiftLog.Pages;
public partial class ScheduleListPage : ContentPage
{
    private readonly ScheduleListViewModel _viewModel;

    public ScheduleListPage(ScheduleListViewModel viewModel)
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