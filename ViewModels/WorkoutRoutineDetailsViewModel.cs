using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutRoutine), nameof(WorkoutRoutine))]
    public partial class WorkoutRoutineDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private WorkoutRoutineListViewModel _workoutRoutineListViewModel;

        private readonly DatabaseContext _context;

        public WorkoutRoutineDetailsViewModel(WorkoutRoutineListViewModel workoutRoutineListViewModel, DatabaseContext context)
        {
            _workoutRoutineListViewModel = workoutRoutineListViewModel;
            _context = context;
        }

        [ObservableProperty]
        private WorkoutRoutine workoutRoutine;

        #nullable enable
        private async Task ExecuteAsync(Func<Task> operation)
        {
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

            }
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task UpdateWorkoutRoutine()
        {
            var (isValid, errorMessage) = WorkoutRoutine.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            if (!await _context.UpdateItemAsync<WorkoutRoutine>(WorkoutRoutine))
            {
                await Shell.Current.DisplayAlert("Error", "Error occured when updating WorkoutRoutine.", "OK");
                return;
            }

            OnPropertyChanged(nameof(WorkoutRoutine));

            WorkoutRoutineListViewModel.IsEditing = false;
        }

        [RelayCommand]
        private async Task VisitSchedule()
        {
            if (WorkoutRoutine is null) return;

            if (WorkoutRoutine.ScheduleId == 0)
            {
                await CreateNewSchedule();
                return;
            }

            try
            {
                if (WorkoutRoutine.IsScheduleWeekly)
                {
                    WeeklySchedule weeklySchedule;

                    try
                    {
                        weeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(WorkoutRoutine.ScheduleId);

                        var navigationParameter = new Dictionary<string, object>
                        {
                            ["WeeklySchedule"] = weeklySchedule
                        };

                        await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={weeklySchedule.Id}", navigationParameter);
                    }
                    catch
                    {
                        await Shell.Current.DisplayAlert("Error", "Weekly Schedule ID does not exist!", "OK");
                        return;
                    }
                } 
                else
                {
                    CustomSchedule customSchedule;

                    try
                    {
                        customSchedule = await _context.GetItemByKeyAsync<CustomSchedule>(WorkoutRoutine.ScheduleId);

                        var navigationParameter = new Dictionary<string, object>
                        {
                            ["CustomSchedule"] = customSchedule
                        };

                        await Shell.Current.GoToAsync($"{nameof(CustomSchedulePage)}?Id={customSchedule.Id}", navigationParameter);
                    }
                    catch
                    {
                        await Shell.Current.DisplayAlert("Error", "Custom Schedule ID does not exist!", "OK");
                        return;
                    }
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Error Loading Schedule!", "OK");
            }
        }

        private async Task CreateNewSchedule()
        {
            string scheduleType = await Shell.Current.DisplayActionSheet("What type of Schedule?", "Cancel", null, "Weekly", "Custom (2-14 days)");
        
            if (scheduleType == "Weekly")
            {
                WeeklySchedule weeklySchedule = new();

                var navigationParameter = new Dictionary<string, object>
                {
                    ["WeeklySchedule"] = weeklySchedule
                };

                await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={weeklySchedule.Id}", navigationParameter);
            }

            if (scheduleType == "Custom")
            {
                CustomSchedule customSchedule = new();

                var navigationParameter = new Dictionary<string, object>
                {
                    ["CustomSchedule"] = customSchedule
                };

                await Shell.Current.GoToAsync($"{nameof(CustomSchedulePage)}?Id={customSchedule.Id}", navigationParameter);

            }
        }

        public async Task LoadWorkoutRoutineSchedule()
        {
            //// Exit function if no Schedule is set
            //if (WorkoutRoutine.ScheduleId == 0) return;

            //ScheduleFactory scheduleFactory = null;

            //await ExecuteAsync(async () =>
            //{
            //    scheduleFactory = await _context.GetItemByKeyAsync<ScheduleFactory>(WorkoutRoutine.ScheduleFactoryId);
            //});

            //if (scheduleFactory is null)
            //{
            //    // Delete ScheduleFactoryId for WorkoutRoutine if ScheduleFactory key does not exist
            //    WorkoutRoutine.ScheduleFactoryId = 0;
            //    await UpdateWorkoutRoutine();
            //}
            //else
            //{
            //    // Set the loaded ScheduleFactory as WorkoutRoutineSchedule

            //    IsScheduleSet = true;
            //}
        }
    }
}
