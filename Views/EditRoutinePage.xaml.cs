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
            if (routine != null) 
            {
                routineCtrl.Name = routine.Name;
                routineCtrl.OrderSlot = routine.OrderSlot.ToString();
            }
            
        }
    }

    private void btnUpdate_Clicked(object sender, EventArgs e)
    {
        routine.Name = routineCtrl.Name;
        routine.OrderSlot = int.Parse(routineCtrl.OrderSlot);

        RoutineRepository.UpdateRoutine(routine.RoutineId, routine);
        Shell.Current.GoToAsync("..");
    }

    private void routineCtrl_OnError(object sender, string e)
    {
        DisplayAlert("Error", e, "OK");
    }
}