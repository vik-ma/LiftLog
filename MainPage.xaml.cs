namespace LocalLiftLog;
using LocalLiftLog.ViewModels;

public partial class MainPage : ContentPage
{
    private readonly RoutineListViewModel _viewModel;
        
    public MainPage(RoutineListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRoutineListAsync();
    }

}