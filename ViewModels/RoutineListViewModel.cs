﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
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
        private ObservableCollection<Routine> _routineList = new();

        [ObservableProperty]
        private Routine _operatingRoutine = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        [ObservableProperty]
        private bool _isEditing = false;

        public async Task LoadRoutinesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var routines = await _context.GetAllAsync<Routine>();
                // Add more models here

                if (routines is not null && routines.Any())
                {
                    routines ??= new ObservableCollection<Routine>();

                    foreach (var routine in routines)
                    {
                        RoutineList.Add(routine);
                    }
                }
            }, "Fetching Routine List...");  
        }

        #nullable enable
        [RelayCommand]
        private void SetOperatingRoutine(Routine? routine)
        {
            OperatingRoutine = routine ?? new();
            IsEditing = true;
        }


        [RelayCommand]
        private async Task SaveRoutineAsync()
        {
            if (OperatingRoutine is null)
                return;

            var (isValid, errorMessage) = OperatingRoutine.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            OperatingRoutine.UpdateDateTime();

            var busyText = OperatingRoutine.Id == 0 ? "Creating Routine List..." : "Updating Routine List";

            await ExecuteAsync(async () =>
            {
                if (OperatingRoutine.Id == 0)
                {
                    // Create Routine List
                    await _context.AddItemAsync<Routine>(OperatingRoutine);
                    RoutineList.Add(OperatingRoutine);
                }
                else
                {
                    // Update Routine List
                    if (await _context.UpdateItemAsync<Routine>(OperatingRoutine))
                    {
                        var routineCopy = OperatingRoutine.Clone();

                        var index = RoutineList.IndexOf(OperatingRoutine);
                        RoutineList.RemoveAt(index);

                        RoutineList.Insert(index, routineCopy);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Product Updation Error", "OK");
                        return;
                    }
                }
                #nullable disable
                SetOperatingRoutineCommand.Execute(new());
            }, busyText);
        }

        [RelayCommand]
        private async Task DeleteRoutineAsync(int id)
        {
            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<Routine>(id))
                {
                    var routine = RoutineList.FirstOrDefault(p => p.Id == id);
                    RoutineList.Remove(routine);
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
                IsEditing = false;
            }
        }

        [RelayCommand]
        private async Task ViewRoutineDetails(int id)
        {
            var routine = RoutineList.FirstOrDefault(p => p.Id == id);

            await Shell.Current.GoToAsync(nameof(RoutineDetailPage));
        }
    }
}
