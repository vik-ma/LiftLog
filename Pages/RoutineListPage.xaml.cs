namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class RoutineListPage : ContentPage
{
    private readonly RoutineListViewModel _viewModel;

    public RoutineListPage(RoutineListViewModel viewModel)
    {
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRoutinesAsync();
    }
}