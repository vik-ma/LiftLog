namespace LocalLiftLog.Pages; 
using LocalLiftLog.ViewModels;

public partial class ProgramDetailsPage : ContentPage
{
    private readonly ProgramDetailsViewModel _viewModel;
    public ProgramDetailsPage(ProgramDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSchedulesAsync();
        await _viewModel.LoadProgramSchedule();
    }
}