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

        [ObservableProperty]
        private Exercise operatingExercise = new();

        [ObservableProperty]
        private ObservableCollection<int> operatingExerciseExerciseGroupIntList = new();

        private CreateExercisePopupPage Popup;

        [ObservableProperty]
        private List<int> pickerExerciseGroupIntList = new();

        [ObservableProperty]
        private bool isEditingExercise;

        public void LoadExerciseList()
        {
            ExerciseList = _exerciseData.ExerciseList;
            FilteredExerciseList = new(ExerciseList);
        }

        public void LoadExerciseGroupIntList()
        {
            ExerciseGroupIntList = new(ExerciseGroupDictionary.GetSortedExerciseGroupList());
            PickerExerciseGroupIntList = new(ExerciseGroupIntList);
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
        public void ResetFilterList()
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

        [RelayCommand]
        private async Task ShowCreateExercisePopup()
        {
            Popup = new CreateExercisePopupPage(this);
            await Shell.Current.ShowPopupAsync(Popup);
        }

        [RelayCommand]
        public void ClosePopup()
        {
            if (Popup is null) return;

            Popup.Close();
        }

        public void AddExerciseGroupToOperatingExercise(int exerciseGroupInt)
        {
            if (OperatingExerciseExerciseGroupIntList.Contains(exerciseGroupInt)) return;

            OperatingExercise.AddExerciseGroup(exerciseGroupInt);
            OperatingExerciseExerciseGroupIntList.Add(exerciseGroupInt);
        }

        [RelayCommand]
        public void RemoveExerciseGroupFromOperatingExercise(int exerciseGroupInt)
        {
            if (!OperatingExerciseExerciseGroupIntList.Contains(exerciseGroupInt)) return;

            OperatingExercise.RemoveExerciseGroup(exerciseGroupInt);
            OperatingExerciseExerciseGroupIntList.Remove(exerciseGroupInt);
        }

        [RelayCommand]
        private async Task EditExercise(Exercise exercise)
        {
            if (exercise is null) return;

            OperatingExercise = exercise;
            OperatingExerciseExerciseGroupIntList = new(exercise.GetExerciseGroupIntList());

            PickerExerciseGroupIntList = new(ExerciseGroupIntList);
            PickerExerciseGroupIntList.RemoveAll(x => OperatingExerciseExerciseGroupIntList.Contains(x));

            IsEditingExercise = true;

            await ShowCreateExercisePopup();
        }

        [RelayCommand]
        private async Task CreateNewExercise()
        {
            if (IsEditingExercise) 
            { 
                // Create brand new objects only if last Exercise was edited
                OperatingExercise = new();
                OperatingExerciseExerciseGroupIntList = new();
                IsEditingExercise = false;
                PickerExerciseGroupIntList = new(ExerciseGroupIntList);
            }

            await ShowCreateExercisePopup();
        }

        [RelayCommand]
        private async Task DeleteExercise()
        {
            if (OperatingExercise is null) return;

            bool userClickedDelete = await Shell.Current.DisplayAlert("Delete Exercise", "Are you sure you want to delete Exercise?", "Delete", "Cancel");

            if (!userClickedDelete) return;

            await _exerciseData.DeleteExerciseAsync(OperatingExercise);

            Popup.Close();

            LoadExerciseList();
        }

        [RelayCommand]
        private async Task SaveExercise()
        {
            var (isExerciseValid, errorMessage) = OperatingExercise.Validate();

            if (!isExerciseValid)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            if (IsEditingExercise) 
            {
                // Update existing Exercise
                await _exerciseData.UpdateExerciseAsync(OperatingExercise);
            }
            else
            {
                // Create new Exercise
                await _exerciseData.CreateExerciseAsync(OperatingExercise);
            }

            Popup.Close();

            LoadExerciseList();
        }
    }
}
