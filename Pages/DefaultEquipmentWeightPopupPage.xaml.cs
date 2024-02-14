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
}