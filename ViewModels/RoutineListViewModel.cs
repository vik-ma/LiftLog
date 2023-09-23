using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using MetalPerformanceShadersGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private ObservableCollection<RoutineList> _routineList;

        [ObservableProperty]
        private RoutineList _operatingRoutineList = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        [RelayCommand]
        private async Task LoadRoutineListAsync()
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

        [RelayCommand]
        private void SetOperatingRoutineList(RoutineList? routineList) => OperatingRoutineList = routineList ?? new();


        [RelayCommand]
        private async Task SaveRoutineListAsync()
        {
            if (OperatingRoutineList is null)
                return;

            var busyText = OperatingRoutineList.Id == 0 ? "Creating Routine List..." : "Updating Product";

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
                    await _context.UpdateItemAsync<RoutineList>(OperatingRoutineList);

                    var routineListCopy = OperatingRoutineList.Clone();

                    var index = RoutineList.IndexOf(OperatingRoutineList);
                    RoutineList.RemoveAt(index);

                    RoutineList.Insert(index, routineListCopy);
                }

                setOperatingRoutineListCommand.Execute(new());
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

        private async Task ExecuteAsync(Func<Task> operation, string? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText ?? "Processing...";
            try
            {
                await operation?.Invoke();
            }
            finally
            {
                IsBusy = false;
                BusyText = "Processing...";
            }
        }
    }
}
