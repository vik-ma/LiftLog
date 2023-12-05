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
    [QueryProperty(nameof(CompletedWorkout), nameof(CompletedWorkout))]
    public partial class StartedWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private CompletedWorkout completedWorkout;

        public StartedWorkoutViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

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

        public async Task LoadWorkoutTemplateAsync()
        {
            if (CompletedWorkout is null) return;

            bool workoutTemplateExists;

            try
            {
                workoutTemplateExists = await _context.ItemExistsByKeyAsync<WorkoutTemplate>(CompletedWorkout.WorkoutTemplateId);

                if (workoutTemplateExists) 
                {
                    WorkoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(CompletedWorkout.WorkoutTemplateId);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Workout Template does not exist!", "OK");
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Error loading Workout Template.", "OK");
                return;
            }
        }

    }

    
}
