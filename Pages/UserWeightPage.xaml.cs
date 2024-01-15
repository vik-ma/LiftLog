namespace LocalLiftLog.Pages;
public partial class UserWeightPage : ContentPage
{
    private readonly UserWeightViewModel _viewModel;
    public UserWeightPage(UserWeightViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUserWeightListAsync();
    }
}