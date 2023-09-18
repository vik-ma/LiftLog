namespace LocalLiftLog.Views;

public partial class AddRoutinePage : ContentPage
{
	public AddRoutinePage()
	{
		InitializeComponent();
	}

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync("..");
    }
}