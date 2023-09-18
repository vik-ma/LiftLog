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
                entryName.Text = routine.Name;
                entryOrderSlot.Text = routine.OrderSlot.ToString();
            }
            
        }
    }

    private void btnUpdate_Clicked(object sender, EventArgs e)
    {
        routine.Name = entryName.Text;
        routine.OrderSlot = int.Parse(entryOrderSlot.Text);

        RoutineRepository.UpdateRoutine(routine.RoutineId, routine);
        Shell.Current.GoToAsync("..");
    }
}