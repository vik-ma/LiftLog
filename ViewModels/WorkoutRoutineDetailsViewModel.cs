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
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutRoutine workoutRoutine;

        public WorkoutRoutineDetailsViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private bool isEditing;

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
            IsEditing = false;
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

                        await GoToWeeklySchedulePage(weeklySchedule);
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

                        await GoToCustomSchedulePage(customSchedule);
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
            string scheduleType = await Shell.Current.DisplayActionSheet("What type of Schedule?", "Cancel", null, "Weekly", "Custom");
        
            if (scheduleType == "Weekly")
            {
                WeeklySchedule weeklySchedule = new() { WorkoutRoutineId = WorkoutRoutine.Id };

                await ExecuteAsync(async () =>
                {
                    if (!await _context.AddItemAsync<WeeklySchedule>(weeklySchedule))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when creating Weekly Schedule.", "OK");
                    }
                    else
                    {
                        WorkoutRoutine.ScheduleId = weeklySchedule.Id;
                        WorkoutRoutine.IsScheduleWeekly = true;
                        await UpdateWorkoutRoutine();
                    }
                });
                
                await GoToWeeklySchedulePage(weeklySchedule);
            }

            if (scheduleType == "Custom")
            {
                string enteredNumber = await Shell.Current.DisplayPromptAsync("Number Of Days In Schedule", "How many days should the schedule contain?\n(Must be between 2 and 14)\n", "OK", "Cancel");

                if (enteredNumber == null) return;

                bool validInput = int.TryParse(enteredNumber, out int numberOfDays);

                if (!validInput || numberOfDays < 2 || numberOfDays > 14)
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid input.", "OK");
                    return;
                }

                CustomSchedule customSchedule = new() 
                { 
                    WorkoutRoutineId = WorkoutRoutine.Id,
                    NumDaysInSchedule = numberOfDays
                };

                await ExecuteAsync(async () =>
                {
                    if (!await _context.AddItemAsync<CustomSchedule>(customSchedule))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when creating Custom Schedule.", "OK");
                    }
                    else
                    {
                        WorkoutRoutine.ScheduleId = customSchedule.Id;
                        WorkoutRoutine.IsScheduleWeekly = false;
                        await UpdateWorkoutRoutine();
                    }
                });

                await GoToCustomSchedulePage(customSchedule);
            }
        }

        private async Task GoToWeeklySchedulePage(WeeklySchedule weeklySchedule)
        {
            if (weeklySchedule is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["WeeklySchedule"] = weeklySchedule
            };

            await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={weeklySchedule.Id}", navigationParameter);

        }

        private async Task GoToCustomSchedulePage(CustomSchedule customSchedule)
        {
            if (customSchedule is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["CustomSchedule"] = customSchedule
            };

            await Shell.Current.GoToAsync($"{nameof(CustomSchedulePage)}?Id={customSchedule.Id}", navigationParameter);
        }

        [RelayCommand]
        private void EnableEditing()
        {
            IsEditing = true;
        }

        [RelayCommand]
        private void DisableEditing()
        {
            IsEditing = false;
        }
    }
}
