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
        public List<Exercise> exerciseList = new();

        [ObservableProperty]
        public ObservableCollection<Exercise> filteredExerciseList = new();

        [ObservableProperty]
        private HashSet<int> exerciseGroupFilterSet = new();

        [ObservableProperty]
        private int exerciseGroupFilterInput;

        [ObservableProperty]
        private List<int> exerciseGroupIntList = new();

        public void LoadExerciseList()
        {
            ExerciseList = _exerciseData.ExerciseList;
            FilteredExerciseList = new(ExerciseList);
        }

        public void LoadExerciseGroupIntList()
        {
            ExerciseGroupIntList = new(ExerciseGroupDictionary.ExerciseGroupDict.Keys);
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
            if (exerciseNum < ConstantsHelper.ExerciseGroupMinValue || exerciseNum > ConstantsHelper.ExerciseGroupMaxValue)
            {
                Shell.Current.DisplayAlert("Error", "Invalid Exercise Group.", "OK");
                return;
            }

            ExerciseGroupFilterSet.Add(exerciseNum);

            FilteredExerciseList = _exerciseData.FilterExerciseListByExerciseGroups(ExerciseGroupFilterSet);

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
