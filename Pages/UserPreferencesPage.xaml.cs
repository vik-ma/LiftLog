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

    private async void OnWeightUnitPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            _viewModel.UserSettings.DefaultWeightUnit = (string)picker.SelectedItem;
            await _viewModel.UpdateUserPreferencesAsync();
        }
    }

    private async void OnDistanceUnitPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            _viewModel.UserSettings.DefaultDistanceUnit = (string)picker.SelectedItem;
            await _viewModel.UpdateUserPreferencesAsync();
        }
    }
}