namespace LocalLiftLog.Pages;

public partial class TimePeriodListPage : ContentPage
{
	private readonly TimePeriodViewModel _viewModel;
	public TimePeriodListPage(TimePeriodViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTimePeriodsAsync();
    }
}