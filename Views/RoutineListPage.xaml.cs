using LocalLiftLog.Models;
using System.Collections.ObjectModel;
//using Routine = LocalLiftLog.Models.Routine;

namespace LocalLiftLog.Views;


public partial class RoutineListPage : ContentPage
{
	//public RoutineListPage()
	//{
	//	InitializeComponent();
	//}

 //   protected override void OnAppearing()
 //   {
 //       base.OnAppearing();

 //       SearchBar.Text = string.Empty;

 //       LoadRoutines();
 //   }

 //   private async void listRoutines_ItemSelected(object sender, SelectedItemChangedEventArgs e)
 //   {
 //       if (listRoutines.SelectedItem != null)
 //       {
 //           await Shell.Current.GoToAsync($"{nameof(EditRoutinePage)}?Id={((Routine)listRoutines.SelectedItem).RoutineId}");
 //       }
 //   }

 //   private void listRoutines_ItemTapped(object sender, ItemTappedEventArgs e)
 //   {
 //       listRoutines.SelectedItem = null;
 //   }

 //   private void btnAdd_Clicked(object sender, EventArgs e)
 //   {
 //       Shell.Current.GoToAsync(nameof(AddRoutinePage));
 //   }

 //   private void Delete_Clicked(object sender, EventArgs e)
 //   {
 //       var menuItem = sender as MenuItem;
 //       var routine = menuItem.CommandParameter as Routine;
 //       RoutineRepository.DeleteRoutine(routine.RoutineId);

 //       LoadRoutines();
 //   }

 //   private void LoadRoutines()
 //   {
 //       var routines = new ObservableCollection<Routine>(RoutineRepository.GetRoutines());

 //       listRoutines.ItemsSource = routines;
 //   }

 //   private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
 //   {
 //       var routines = new ObservableCollection<Routine>(RoutineRepository.SearchRoutines(((SearchBar)sender).Text));

 //       listRoutines.ItemsSource = routines;
 //   }
}