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
        _viewModel.LoadDefaultWeightUnit();
    }

    private void OnWeightUnitPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            _viewModel.SelectedWeightUnit = (string)picker.SelectedItem;
        }
    }
}