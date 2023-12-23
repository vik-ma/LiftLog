using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Helpers;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    public partial class SelectWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        private ScheduleFactory OperatingSchedule;

        private WorkoutRoutine OperatingRoutine;

        public SelectWorkoutViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<ScheduleFactory> _scheduleList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutRoutine> _workoutRoutineList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> _workoutList = new();

        [ObservableProperty]
        private DateTime selectedDate;

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
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

        public async Task LoadRoutinesAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutRoutineList.Clear();

                var routines = await _context.GetAllAsync<WorkoutRoutine>();

                if (routines is not null && routines.Any())
                {
                    routines ??= new ObservableCollection<WorkoutRoutine>();

                    foreach (var routine in routines)
                    {
                        WorkoutRoutineList.Add(routine);
                    }
                }
            });
        }

        public async Task LoadSchedulesAsync()
        {
            await ExecuteAsync(async () =>
            {
                ScheduleList.Clear();

                var schedules = await _context.GetAllAsync<ScheduleFactory>();

                if (schedules is not null && schedules.Any())
                {
                    schedules ??= new ObservableCollection<ScheduleFactory>();

                    foreach (var schedule in schedules)
                    {
                        ScheduleList.Add(schedule);
                    }
                }
            });
        }

        public async Task LoadWorkoutsFromOperatingScheduleAsync()
        {
            // Exit function if no OperatingSchedule is set
            if (OperatingSchedule is null) return;

            WorkoutList.Clear();

            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.ScheduleFactoryId == OperatingSchedule.Id;

            try
            {
                var filteredList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);

                foreach (var item in filteredList)
                {
                    WorkoutList.Add(item);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                return;
            }
        }

        [RelayCommand]
        private async Task SetOperatingSchedule(int id)
        {
            var schedule = ScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null) return;

            OperatingSchedule = schedule;

            await LoadWorkoutsFromOperatingScheduleAsync();
        }

        public async Task LoadWorkoutsFromOperatingRoutineAsync()
        {
            // Exit function if no OperatingRoutine is set
            if (OperatingRoutine is null) return;

            WorkoutList.Clear();

            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == OperatingRoutine.Id;

            try
            {
                var filteredList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);

                foreach (var item in filteredList)
                {
                    WorkoutList.Add(item);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                return;
            }
        }

        [RelayCommand]
        private async Task SetOperatingRoutine(int id)
        {
            var routine = WorkoutRoutineList.FirstOrDefault(p => p.Id == id);

            if (routine is null) return;

            OperatingRoutine = routine;

            await LoadWorkoutsFromOperatingRoutineAsync();
        }

        [RelayCommand]
        private async Task GoToSchedulePage(int id)
        {
            var schedule = ScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null)
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist", "OK");
                return;
            }

            if (schedule.IsScheduleWeekly)
            {
                WeeklySchedule weeklySchedule;

                try
                {
                    weeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(schedule.ScheduleId);
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Error", "Weekly Schedule ID does not exist!", "OK");
                    return;
                }

                var navigationParameter = new Dictionary<string, object>
                {
                    ["WeeklySchedule"] = weeklySchedule
                };

                await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={id}", navigationParameter);
            }
            else
            {
                CustomSchedule customSchedule;

                try
                {
                    customSchedule = await _context.GetItemByKeyAsync<CustomSchedule>(schedule.ScheduleId);
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Error", "Custom Schedule ID does not exist!", "OK");
                    return;
                }

                var navigationParameter = new Dictionary<string, object>
                {
                    ["CustomSchedule"] = customSchedule
                };

                await Shell.Current.GoToAsync($"{nameof(CustomSchedulePage)}?Id={id}", navigationParameter);
            }
        }

        [RelayCommand]
        private async Task DeleteScheduleAsync(int id)
        {
            var schedule = ScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null)
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist.", "OK");
                return;
            };

            await ExecuteAsync(async () =>
            {
                if (schedule.IsScheduleWeekly)
                {
                    if (!await _context.DeleteItemByKeyAsync<WeeklySchedule>(schedule.ScheduleId))
                    {
                        await Shell.Current.DisplayAlert("Delete Error", "Weekly Schedule was not deleted.", "OK");
                    }
                }
                else
                {
                    if (!await _context.DeleteItemByKeyAsync<CustomSchedule>(schedule.ScheduleId))
                    {
                        await Shell.Current.DisplayAlert("Delete Error", "Custom Schedule was not deleted.", "OK");
                    }
                }

                if (await _context.DeleteItemByKeyAsync<ScheduleFactory>(id))
                {
                    ScheduleList.Remove(schedule);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Schedule was not deleted.", "OK");
                }
            });

            await DeleteWorkoutTemplateCollectionsByScheduleFactoryId(id);
        }

        private async Task DeleteWorkoutTemplateCollectionsByScheduleFactoryId(int scheduleId)
        {
            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.ScheduleFactoryId == scheduleId;

            IEnumerable<WorkoutTemplateCollection> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Workout Template Collections.", "OK");
            }

            foreach (var item in filteredList)
            {
                await DeleteWorkoutTemplateCollection(item);
            }
        }

        [RelayCommand]
        private async Task DeleteWorkoutTemplateCollection(WorkoutTemplateCollection workoutTemplateCollection)
        {
            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<WorkoutTemplateCollection>(workoutTemplateCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                    return;
                }

                WorkoutList.Remove(workoutTemplateCollection);
            });
        }

        [RelayCommand]
        private async Task GoToWorkoutDetailsPage(int id)
        {
            WorkoutTemplateCollection workoutTemplateCollection = WorkoutList.FirstOrDefault(p => p.Id == id);

            if (workoutTemplateCollection is null)
            {
                await Shell.Current.DisplayAlert("Error", "Workout does not exist", "OK");
                return;
            }

            WorkoutTemplate workoutTemplate;

            try
            {
                workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(workoutTemplateCollection.WorkoutTemplateId);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Workout Template ID does not exist!", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutTemplate"] = workoutTemplate
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?Id={id}", navigationParameter);
        }

        [RelayCommand]
        private async Task GoToScheduleListPage()
        {
            await Shell.Current.GoToAsync(nameof(ScheduleListPage));
        }

        [RelayCommand]
        private async Task StartWorkout(int id)
        {
            WorkoutTemplateCollection workoutTemplateCollection = WorkoutList.FirstOrDefault(p => p.Id == id);

            if (workoutTemplateCollection is null) return;

            WorkoutTemplate workoutTemplate;

            try
            {
                workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(workoutTemplateCollection.WorkoutTemplateId);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Workout Template ID does not exist!", "OK");
                return;
            }

            CompletedWorkout completedWorkout = await GetCompletedWorkoutAtDate(workoutTemplateCollection.Id, workoutTemplate.Id, SelectedDate)
                                                ?? new() { 
                                                    WorkoutTemplateId = workoutTemplate.Id,
                                                    WorkoutTemplateCollectionId = workoutTemplateCollection.Id,
                                                };

            var navigationParameter = new Dictionary<string, object>
            {
                ["CompletedWorkout"] = completedWorkout
            };

            await Shell.Current.GoToAsync($"{nameof(StartedWorkoutPage)}?Id={id}", navigationParameter);
        }

        private async Task<CompletedWorkout> GetCompletedWorkoutAtDate(int workoutTemplateCollectionId, int workoutTemplateId, DateTime dateTime)
        {
            string ymdDate = DateTimeHelper.FormatDateTimeToYmdString(dateTime);

            Expression<Func<CompletedWorkout, bool>> predicate = entity => entity.WorkoutTemplateId == workoutTemplateId && entity.WorkoutTemplateCollectionId == workoutTemplateCollectionId && entity.Date == ymdDate;

            try
            {
                var filteredList = await _context.GetFilteredAsync<CompletedWorkout>(predicate);

                if (filteredList.Count() == 1)
                {
                    return filteredList.Single();
                }
                else return null;
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Error occured when trying to fetch Workout.", "OK");
                return null;
            }
        }
    }
}
