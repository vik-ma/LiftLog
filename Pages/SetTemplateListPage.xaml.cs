namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;


public partial class SetTemplateListPage : ContentPage
{
	private readonly SetTemplateViewModel _viewModel;
	public SetTemplateListPage(SetTemplateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSetTemplatesAsync();
    }
}