namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class SetListDetailsPage : ContentPage
{
    private readonly SetListDetailsViewModel _viewModel;
    public SetListDetailsPage(SetListDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
    }
}