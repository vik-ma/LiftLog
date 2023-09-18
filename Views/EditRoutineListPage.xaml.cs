namespace LocalLiftLog.Views;

public partial class EditRoutineListPage : ContentPage
{
	public EditRoutineListPage()
	{
		InitializeComponent();
	}

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}