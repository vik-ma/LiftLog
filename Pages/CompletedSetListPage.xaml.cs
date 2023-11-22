namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class CompletedSetListPage : ContentPage
{
	private readonly CompletedSetViewModel _viewModel;
	public CompletedSetListPage(CompletedSetViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCompletedSetsAsync();
    }
}