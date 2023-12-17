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
        private bool isShowingScheduleList = false;

        [ObservableProperty]
        private WorkoutRoutine workoutRoutine;

        [ObservableProperty]
        private ObservableCollection<ScheduleFactory> _scheduleFactoryList = new();

        [ObservableProperty]
        private ScheduleFactory workoutRoutineSchedule;

        [ObservableProperty]
        private bool isScheduleSet = false;

        public async Task LoadSchedulesAsync()
        {
            await ExecuteAsync(async () =>
            {
                ScheduleFactoryList.Clear();

                var schedules = await _context.GetAllAsync<ScheduleFactory>();

                if (schedules is not null && schedules.Any())
                {
                    schedules ??= new ObservableCollection<ScheduleFactory>();

                    foreach (var schedule in schedules)
                    {
                        ScheduleFactoryList.Add(schedule);
                    }
                }
            });
        }

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
            var ScheduleFactoryId = WorkoutRoutine.ScheduleFactoryId;

            if (ScheduleFactoryId == 0)
            {
                await Shell.Current.DisplayAlert("Error", "No Schedule Is Set!", "OK");
                return;
            }

            try
            {
                var schedule = await _context.GetItemByKeyAsync<ScheduleFactory>(ScheduleFactoryId);

                if (schedule.IsScheduleWeekly)
                {
                    WeeklySchedule weeklySchedule;

                    try
                    {
                        weeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(schedule.ScheduleId);

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
                        customSchedule = await _context.GetItemByKeyAsync<CustomSchedule>(schedule.ScheduleId);

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
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist!", "OK");
            }
        }

        [RelayCommand]
        private void ShowScheduleList()
        {
            IsShowingScheduleList = true;
        }

        public async Task LoadWorkoutRoutineSchedule()
        {
            // Exit function if no ScheduleFactory is set
            if (WorkoutRoutine.ScheduleFactoryId == 0) return;

            ScheduleFactory scheduleFactory = null;

            await ExecuteAsync(async () =>
            {
                scheduleFactory = await _context.GetItemByKeyAsync<ScheduleFactory>(WorkoutRoutine.ScheduleFactoryId);
            });

            if (scheduleFactory is null)
            {
                // Delete ScheduleFactoryId for WorkoutRoutine if ScheduleFactory key does not exist
                WorkoutRoutine.ScheduleFactoryId = 0;
                await UpdateWorkoutRoutine();
            }
            else
            {
                // Set the loaded ScheduleFactory as WorkoutRoutineSchedule
                WorkoutRoutineSchedule = scheduleFactory;
                IsScheduleSet = true;
            }
        }

        [RelayCommand]
        private async Task SetSchedule(ScheduleFactory schedule)
        {
            if (schedule == null) return;

            WorkoutRoutine.ScheduleFactoryId = schedule.Id;

            await UpdateWorkoutRoutine();
            await LoadWorkoutRoutineSchedule();

            IsShowingScheduleList = false;
            IsScheduleSet = true;
        }

        [RelayCommand]
        private async Task GoToScheduleList()
        {
            await Shell.Current.GoToAsync($"{nameof(ScheduleListPage)}");
        }
    }
}
