using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using System.Collections.ObjectModel;

namespace LocalLiftLog.ViewModels
{
    public partial class RoutineListViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public RoutineListViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<RoutineList> _routineList = new();

        [ObservableProperty]
        private RoutineList _operatingRoutineList = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        public async Task LoadRoutineListAsync()
        {
            await ExecuteAsync(async () =>
            {
                var routineList = await _context.GetAllAsync<RoutineList>();
                // Add more models here

                if (routineList is not null && routineList.Any())
                {
                    routineList ??= new ObservableCollection<RoutineList>();

                    foreach (var routine in routineList)
                    {
                        RoutineList.Add(routine);
                    }
                }
            }, "Fetching Routine List...");  
        }

        #nullable enable
        [RelayCommand]
        private void SetOperatingRoutineList(RoutineList? routineList) => OperatingRoutineList = routineList ?? new();


        [RelayCommand]
        private async Task SaveRoutineListAsync()
        {
            if (OperatingRoutineList is null)
                return;

            var (isValid, errorMessage) = OperatingRoutineList.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            var busyText = OperatingRoutineList.Id == 0 ? "Creating Routine List..." : "Updating Routine List";

            await ExecuteAsync(async () =>
            {
                if (OperatingRoutineList.Id == 0)
                {
                    // Create Routine List
                    await _context.AddItemAsync<RoutineList>(OperatingRoutineList);
                    RoutineList.Add(OperatingRoutineList);
                }
                else
                {
                    // Update Routine List
                    if (await _context.UpdateItemAsync<RoutineList>(OperatingRoutineList))
                    {
                        var routineListCopy = OperatingRoutineList.Clone();

                        var index = RoutineList.IndexOf(OperatingRoutineList);
                        RoutineList.RemoveAt(index);

                        RoutineList.Insert(index, routineListCopy);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Product Updation Error", "OK");
                        return;
                    }
                }
                #nullable disable
                SetOperatingRoutineListCommand.Execute(new());
            }, busyText);
        }

        [RelayCommand]
        private async Task DeleteRoutineListAsync(int id)
        {
            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<RoutineList>(id))
                {
                    var routineList = RoutineList.FirstOrDefault(p => p.Id == id);
                    RoutineList.Remove(routineList);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Routine List was not deleted.", "OK");
                }
            }, "Deleting Routine List...");
        }

        #nullable enable
        private async Task ExecuteAsync(Func<Task> operation, string? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText ?? "Processing...";
            try
            {
                #nullable disable
                await operation?.Invoke();
            }
            catch
            {

            }
            finally
            {
                IsBusy = false;
                BusyText = "Processing...";
            }
        }
    }
}
