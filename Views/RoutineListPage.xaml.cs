using LocalLiftLog.Models;
using System.Collections.ObjectModel;
using Routine = LocalLiftLog.Models.Routine;

namespace LocalLiftLog.Views;


public partial class RoutineListPage : ContentPage
{
	public RoutineListPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var routines = new ObservableCollection<Routine>(RoutineRepository.GetRoutines());

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

    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(AddRoutinePage));
    }
}