using LocalLiftLog.ViewModels;

namespace LocalLiftLog.Pages;

public partial class RoutineDetailPage : ContentPage
{
	public RoutineDetailPage(RoutineDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}