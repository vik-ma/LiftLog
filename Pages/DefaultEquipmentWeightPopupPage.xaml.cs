namespace LocalLiftLog.Pages;

public partial class DefaultEquipmentWeightPopupPage : Popup
{
	private readonly UserPreferencesViewModel _viewModel;
	public DefaultEquipmentWeightPopupPage(UserPreferencesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
    
    private void OnWeightUnitPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            _viewModel.OperatingDefaultEquipmentWeight.WeightUnit = (string)picker.SelectedItem;
        }
    }
}