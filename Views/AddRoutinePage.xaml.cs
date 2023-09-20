using LocalLiftLog.Models;

namespace LocalLiftLog.Views;

public partial class AddRoutinePage : ContentPage
{
	public AddRoutinePage()
	{
		InitializeComponent();
	}

    private void routineCtrl_OnSave(object sender, EventArgs e)
    {
        RoutineRepository.AddRoutine(new Models.Routine
        {
            Name = routineCtrl.Name,
            OrderSlot = int.Parse(routineCtrl.OrderSlot)
        });

        Shell.Current.GoToAsync($"//{nameof(RoutineListPage)}");
    }

    private void routineCtrl_OnCancel(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync($"//{nameof(RoutineListPage)}");
    }

    private void routineCtrl_OnError(object sender, string e)
    {
        DisplayAlert("Error", e, "OK");
    }
}