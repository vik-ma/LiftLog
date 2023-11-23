using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using LocalLiftLog.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq.Expressions;

namespace LocalLiftLog.ViewModels
{
    public partial class CompletedWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public CompletedWorkoutViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<CompletedWorkout> completedWorkoutList = new();

        public async Task LoadCompletedWorkoutsAsync()
        {
            await ExecuteAsync(async () =>
            {
                CompletedWorkoutList.Clear();

                var completedWorkouts = await _context.GetAllAsync<CompletedWorkout>();

                if (completedWorkouts is not null && completedWorkouts.Any())
                {
                    completedWorkouts ??= new ObservableCollection<CompletedWorkout>();

                    foreach (var set in completedWorkouts)
                    {
                        CompletedWorkoutList.Add(set);
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
        private async Task CreateCompletedWorkoutAsync()
        {
            await ExecuteAsync(async () =>
            {
                CompletedWorkout set = new();
                await _context.AddItemAsync<CompletedWorkout>(set);
                CompletedWorkoutList.Add(set);
            });
        }

        [RelayCommand]
        private async Task UpdateCompletedWorkoutAsync(int id)
        {
            CompletedWorkout completedWorkout = CompletedWorkoutList.FirstOrDefault(p => p.Id == id);

            if (completedWorkout is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CompletedWorkout>(completedWorkout))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set.", "OK");
                }

                await LoadCompletedWorkoutsAsync();
            });
        }

        [RelayCommand]
        private async Task DeleteCompletedWorkoutAsync(int id)
        {
            CompletedWorkout completedWorkout = CompletedWorkoutList.FirstOrDefault(p => p.Id == id);

            if (completedWorkout is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<CompletedWorkout>(completedWorkout))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set.", "OK");
                }
            });

            await DeleteCompletedSetsByCompletedWorkoutId(id);

            await LoadCompletedWorkoutsAsync();
        }

        [RelayCommand]
        private async Task MarkCompletedWorkoutAsComplete(int id)
        {
            CompletedWorkout completedWorkout = CompletedWorkoutList.FirstOrDefault(p => p.Id == id);

            if (completedWorkout is null)
                return;

            string currentDateTimeString = DateTimeHelper.GetCurrentFormattedDateTime();

            completedWorkout.IsCompleted = true;
            completedWorkout.DateCompleted = currentDateTimeString;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CompletedWorkout>(completedWorkout))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set.", "OK");
                }

                await LoadCompletedWorkoutsAsync();
            });
        }

        private async Task DeleteCompletedSetsByCompletedWorkoutId(int id)
        {
            Expression<Func<CompletedSet, bool>> predicate = entity => entity.CompletedWorkoutId == id;

            IEnumerable<CompletedSet> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<CompletedSet>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Workout Template Collections.", "OK");
            }

            foreach (var item in filteredList)
            {
                await ExecuteAsync(async () =>
                {
                    if (!await _context.DeleteItemAsync<CompletedSet>(item))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                    }
                });
            }
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
