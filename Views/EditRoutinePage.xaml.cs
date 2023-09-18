namespace LocalLiftLog.Views;

public partial class EditRoutinePage : ContentPage
{
	public EditRoutinePage()
	{
		InitializeComponent();
	}

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}