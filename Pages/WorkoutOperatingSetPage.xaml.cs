namespace LocalLiftLog.Pages;

public partial class WorkoutOperatingSetPage : ContentPage
{
    private readonly WorkoutViewModel _viewModel;
    public WorkoutOperatingSetPage(WorkoutViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}