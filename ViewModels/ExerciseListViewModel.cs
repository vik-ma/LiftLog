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

        [ObservableProperty]
        private List<int> exerciseGroupIntList = new();

        public async Task LoadExercisesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var exercises = await _exerciseData.GetFullExerciseList();

                ExerciseList.Clear();

                if (exercises is not null && exercises.Any())
                {
                    exercises ??= new ObservableCollection<Exercise>();

                    foreach (var exercise in exercises)
                    {
                        ExerciseList.Add(exercise);
                    }
                }

                FilteredExerciseList = new(ExerciseList);
            });
        }

        public void LoadExerciseGroupIntList()
        {
            ExerciseGroupIntList = new(ExerciseGroupDictionary.ExerciseGroupDict.Keys);
        }

        private void UpdateFilteredExerciseList(IEnumerable<Exercise> exercises)
        {
            FilteredExerciseList.Clear();

            if (exercises is not null && exercises.Any())
            {
                exercises ??= new ObservableCollection<Exercise>();

                foreach (var exercise in exercises)
                {
                    FilteredExerciseList.Add(exercise);
                }
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

        public void AddExerciseGroupToFilterList(int exerciseNum)
        {
            if (exerciseNum < 0 || exerciseNum > exerciseGroupDict.Count)
            {
                Shell.Current.DisplayAlert("Error", "Invalid Exercise Group.", "OK");
                return;
            }

            ExerciseGroupFilterSet.Add(exerciseNum);

            var exercises = _exerciseData.FilterExerciseListByExerciseGroups(ExerciseGroupFilterSet);

            UpdateFilteredExerciseList(exercises);

            OnPropertyChanged(nameof(ExerciseGroupFilterSet));
        }

        [RelayCommand]
        private void ResetFilterList()
        {
            if (ExerciseGroupFilterSet.Count == 0) return;

            ExerciseGroupFilterSet.Clear();

            FilteredExerciseList = new(ExerciseList);

            OnPropertyChanged(nameof(ExerciseGroupFilterSet));
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
