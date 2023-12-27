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

        private readonly Dictionary<int, string> exerciseGroupDict;

        public ExerciseListViewModel(ExerciseDataManager exerciseData)
        {
            _exerciseData = exerciseData;
            exerciseGroupDict = ExerciseGroupDictionary.ExerciseGroupDict;
        }

        [ObservableProperty]
        public List<Exercise> exerciseList = new();

        [ObservableProperty]
        public ObservableCollection<Exercise> filteredExerciseList = new();

        [ObservableProperty]
        private HashSet<int> exerciseGroupFilterSet = new();

        [ObservableProperty]
        private int exerciseGroupFilterInput;

        public async Task LoadExercisesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var exercises = await _exerciseData.GetFullExerciseList();

                ExerciseList.Clear();

                UpdateExerciseList(exercises);
            });
        }

        private void UpdateExerciseList(IEnumerable<Exercise> exercises)
        {
            if (exercises is not null && exercises.Any())
            {
                exercises ??= new ObservableCollection<Exercise>();

                foreach (var exercise in exercises)
                {
                    ExerciseList.Add(exercise);
                }

                FilteredExerciseList = new(ExerciseList);
            }
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
        private void AddExerciseGroupToFilterList(int exerciseNum)
        {
            if (exerciseNum < 0 || exerciseNum > exerciseGroupDict.Count)
            {
                Shell.Current.DisplayAlert("Error", "Invalid Exercise Group.", "OK");
                return;
            }

            ExerciseGroupFilterSet.Add(exerciseNum);

            ExerciseList.Clear();

            var exercises = _exerciseData.FilterExerciseListByExerciseGroups(ExerciseGroupFilterSet);

            UpdateExerciseList(exercises);

            OnPropertyChanged(nameof(ExerciseGroupFilterSet));
        }

        [RelayCommand]
        private async Task ResetFilterList()
        {
            if (ExerciseGroupFilterSet.Count == 0) return;

            ExerciseGroupFilterSet.Clear();

            ExerciseList.Clear();

            await LoadExercisesAsync();

            OnPropertyChanged(nameof(ExerciseGroupFilterSet));
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
