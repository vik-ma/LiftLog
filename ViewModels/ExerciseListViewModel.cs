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
    public partial class ExerciseListViewModel : ObservableObject
    {
        private readonly ExerciseDataManager _exerciseData;

        public ExerciseListViewModel(ExerciseDataManager exerciseData)
        {
            _exerciseData = exerciseData;
        }

        [ObservableProperty]
        private ObservableCollection<Exercise> exerciseList = new();

        public async Task LoadExercisesAsync()
        {
            await ExecuteAsync(async () =>
            {
                ExerciseList.Clear();

                var exercises = await _exerciseData.GetFullExerciseList();

                if (exercises is not null && exercises.Any())
                {
                    exercises ??= new ObservableCollection<Exercise>();

                    foreach (var exercise in exercises)
                    {
                        ExerciseList.Add(exercise);
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
    }
}
