namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class ProgramListPage : ContentPage
{
    private readonly ProgramListViewModel _viewModel;

    public ProgramListPage(ProgramListViewModel viewModel)
    {
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProgramsAsync();
    }
}