namespace LocalLiftLog.Pages;
public partial class UserPreferencesPage : ContentPage
{
	private readonly UserPreferencesViewModel _viewModel;
	public UserPreferencesPage(UserPreferencesViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}