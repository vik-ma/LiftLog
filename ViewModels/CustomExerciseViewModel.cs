using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq.Expressions;

namespace LocalLiftLog.ViewModels
{
    public partial class CustomExerciseViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;
        private readonly ExerciseDataManager _exerciseData;

        public CustomExerciseViewModel(DatabaseContext context, ExerciseDataManager exerciseData)
        {
            _context = context;
            _exerciseData = exerciseData;
        }

        [ObservableProperty]
        private ObservableCollection<CustomExercise> customExerciseList = new();

        public async Task LoadCustomExercisesAsync()
        {
            await ExecuteAsync(async () =>
            {
                CustomExerciseList.Clear();

                var customExercise = await _context.GetAllAsync<CustomExercise>();

                if (customExercise is not null && customExercise.Any())
                {
                    customExercise ??= new ObservableCollection<CustomExercise>();

                    foreach (var exercise in customExercise)
                    {
                        CustomExerciseList.Add(exercise);
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
        private async Task CreateCustomExerciseAsync()
        {
            await ExecuteAsync(async () =>
            {
                CustomExercise exercise = new();
                await _context.AddItemAsync<CustomExercise>(exercise);
                CustomExerciseList.Add(exercise);
            });
        }

        [RelayCommand]
        private async Task UpdateCustomExerciseAsync(int id)
        {
            CustomExercise customExercise = CustomExerciseList.FirstOrDefault(p => p.Id == id);

            if (customExercise is null)
                return;

            if (await _exerciseData.ValidateUniqueExerciseName(customExercise.Name))
            {
                await Shell.Current.DisplayAlert("Error", "Exercise Name must be unique.", "OK");
                return;
            }

            if (!customExercise.ValidateExerciseGroups())
            {
                await Shell.Current.DisplayAlert("Error", "At least one Exercise Group must be added.", "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CustomExercise>(customExercise))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Custom Exercise.", "OK");
                }

                await LoadCustomExercisesAsync();
            });
        }

        [RelayCommand]
        private async Task DeleteCustomExerciseAsync(int id)
        {
            CustomExercise customExercise = CustomExerciseList.FirstOrDefault(p => p.Id == id);

            if (customExercise is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<CustomExercise>(customExercise))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Custom Exercise.", "OK");
                }
            });

            await LoadCustomExercisesAsync();
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
