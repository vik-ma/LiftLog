using LocalLiftLog.Models;
using Routine = LocalLiftLog.Models.Routine;

namespace LocalLiftLog.Views;


public partial class RoutineListPage : ContentPage
{
	public RoutineListPage()
	{
		InitializeComponent();

        List<Routine> routines = RoutineRepository.GetRoutines();

        listRoutines.ItemsSource = routines;
        
	}

    private async void listRoutines_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (listRoutines.SelectedItem != null)
        {
            await Shell.Current.GoToAsync($"{nameof(EditRoutinePage)}?Id={((Routine)listRoutines.SelectedItem).RoutineId}");
        }
    }

    private void listRoutines_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        listRoutines.SelectedItem = null;
    }

    //private void btnEditRoutineList_Clicked(object sender, EventArgs e)
    //{
    //    Shell.Current.GoToAsync(nameof(EditRoutineListPage));
    //}

    //private void btnAddRoutine_Clicked(object sender, EventArgs e)
    //{
    //    Shell.Current.GoToAsync(nameof(AddRoutinePage));
    //}

    //private void btnEditRoutine_Clicked(object sender, EventArgs e)
    //{
    //    Shell.Current.GoToAsync(nameof(EditRoutinePage));
    //}
}