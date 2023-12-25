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

        [ObservableProperty]
        private WorkoutRoutine operatingRoutine;
        public SelectWorkoutViewModel(DatabaseContext context)
        {
            _context = context;
        }

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
        private async Task DeleteRoutineAsync(int id)
        {
            var routine = WorkoutRoutineList.FirstOrDefault(p => p.Id == id);

            if (routine is null) return;

            bool isScheduleWeekly = routine.IsScheduleWeekly;

            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<WorkoutRoutine>(id))
                {
                    WorkoutRoutineList.Remove(routine);

                    if (routine == OperatingRoutine)
                    {
                        OperatingRoutine = new();
                        WorkoutList.Clear();
                    }

                    // Delete WorkoutTemplateCollection objects referencing the WorkoutRoutine
                    await DeleteWorkoutTemplateCollectionsByWorkoutRoutineId(id);

                    // Delete Schedule objects referencing the WorkoutRoutine
                    if (isScheduleWeekly) await DeleteWeeklyScheduleByWorkoutRoutineId(id);
                    else await DeleteCustomScheduleByWorkoutRoutineId(id);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "An error occured when trying to delete Workout Routine.", "OK");
                }
            });
        }

        private async Task DeleteWorkoutTemplateCollectionsByWorkoutRoutineId(int routineId)
        {
            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == routineId;

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

        private async Task DeleteWeeklyScheduleByWorkoutRoutineId(int routineId)
        {
            Expression<Func<WeeklySchedule, bool>> predicate = entity => entity.WorkoutRoutineId == routineId;

            IEnumerable<WeeklySchedule> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<WeeklySchedule>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Weekly Schedules.", "OK");
            }

            foreach (var item in filteredList)
            {
                await ExecuteAsync(async () =>
                {
                    if (!await _context.DeleteItemAsync<WeeklySchedule>(item))
                    {
                        await Shell.Current.DisplayAlert("Error", "An error occured when trying to delete Weekly Schedule.", "OK");
                    }
                });
            }
        }

        private async Task DeleteCustomScheduleByWorkoutRoutineId(int routineId)
        {
            Expression<Func<CustomSchedule, bool>> predicate = entity => entity.WorkoutRoutineId == routineId;

            IEnumerable<CustomSchedule> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<CustomSchedule>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Custom Schedules.", "OK");
            }

            foreach (var item in filteredList)
            {
                await ExecuteAsync(async () =>
                {
                    if (!await _context.DeleteItemAsync<CustomSchedule>(item))
                    {
                        await Shell.Current.DisplayAlert("Error", "An error occured when trying to delete Custom Schedule.", "OK");
                    }
                });
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
        private async Task GoToWorkoutRoutineDetailsPage(int id)
        {
            WorkoutRoutine workoutRoutine = WorkoutRoutineList.FirstOrDefault(p => p.Id == id);

            if (workoutRoutine is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutRoutine"] = workoutRoutine
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutRoutineDetailsPage)}?Id={id}", navigationParameter);
        }

        [RelayCommand]
        private async Task GoToWorkoutRoutineListPage()
        {
            await Shell.Current.GoToAsync(nameof(WorkoutRoutineListPage));
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

        [RelayCommand]
        private async Task CreateWorkoutTemplateAsync()
        {
            WorkoutTemplate workout = new();

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<WorkoutTemplate>(workout);
            });

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutTemplate"] = workout
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?Id={workout.Id}", navigationParameter);
        }
    }
}
