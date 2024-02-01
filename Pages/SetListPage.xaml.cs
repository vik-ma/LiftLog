namespace LocalLiftLog.Pages;
partial class SetListPage : ContentPage
{
	private readonly SetListViewModel _viewModel;
	public SetListPage(SetListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSetsAsync();
    }
}