using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace LocalLiftLog.ViewModels
{
    public partial class WorkoutTemplateViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public WorkoutTemplateViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> _workoutTemplateList = new();

        [ObservableProperty]
        private bool _isCreating = false;

        [RelayCommand]
        private void CancelCreating()
        {
            IsCreating = false;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task LoadWorkoutTemplatesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var workouts = await _context.GetAllAsync<WorkoutTemplate>();

                if (workouts is not null && workouts.Any())
                {
                    WorkoutTemplateList.Clear();

                    workouts ??= new ObservableCollection<WorkoutTemplate>();

                    foreach (var workout in workouts)
                    {
                        WorkoutTemplateList.Add(workout);
                    }
                }
            });
        }

        [RelayCommand]
        private async Task CreateWorkoutTemplateAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutTemplate workout = new();
                await _context.AddItemAsync<WorkoutTemplate>(workout);
                WorkoutTemplateList.Add(workout);
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

    }
}
