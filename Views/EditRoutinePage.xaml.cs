using LocalLiftLog.Models;
using Routine = LocalLiftLog.Models.Routine;

namespace LocalLiftLog.Views;

[QueryProperty(nameof(RoutineId), "Id")]

public partial class EditRoutinePage : ContentPage
{
    private Routine routine;
	public EditRoutinePage()
	{
		InitializeComponent();
	}

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }

    public string RoutineId
    {
        set
        {
            routine = RoutineRepository.GetRoutineById(int.Parse(value));
            lblName.Text = routine.Name;
        }
    }
}