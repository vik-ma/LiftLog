namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class CustomSchedulePage : ContentPage
{
    private readonly CustomScheduleViewModel _viewModel;

    public CustomSchedulePage(CustomScheduleViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWorkoutTemplateCollectionsAsync();
        await _viewModel.LoadWorkoutTemplatesAsync();
    }
}