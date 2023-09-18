namespace LocalLiftLog.Views;

public partial class RoutineListPage : ContentPage
{
	public RoutineListPage()
	{
		InitializeComponent();
	}

    private void btnEditRoutineList_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(EditRoutineListPage));
    }

    private void btnAddRoutine_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(AddRoutinePage));
    }

    private void btnEditRoutine_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(EditRoutinePage));
    }
}