namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class RoutineSchedulePage : ContentPage
{
    public RoutineSchedulePage(RoutineScheduleViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}