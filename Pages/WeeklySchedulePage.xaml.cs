namespace LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;

public partial class WeeklySchedulePage : ContentPage
{
    public WeeklySchedulePage(WeeklyScheduleViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}