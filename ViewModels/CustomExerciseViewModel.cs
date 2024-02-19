namespace LocalLiftLog.ViewModels
{
    public partial class CustomExerciseViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        private readonly ExerciseDataManager _exerciseData;

        [ObservableProperty]
        private ObservableCollection<Exercise> exerciseList = [];

        public CustomExerciseViewModel(DatabaseContext context, ExerciseDataManager exerciseData)
        {
            _context = context;
            _exerciseData = exerciseData;
        }

        [ObservableProperty]
        private ObservableCollection<Exercise> defaultExerciseList = new(DefaultExercises.DefaultExerciseList);

        [ObservableProperty]
        private ObservableCollection<Exercise> filteredDefaultExerciseList = [];

        [ObservableProperty]
        private bool displayDefaultExerciseList = false;

        public void LoadExerciseList()
        {
            ExerciseList = new ObservableCollection<Exercise>(_exerciseData.ExerciseList);
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
        private async Task CreateExerciseAsync()
        {
            string enteredName = await Shell.Current.DisplayPromptAsync("Enter Name", "Enter Exercise Name", "OK", "Cancel");

            if (string.IsNullOrWhiteSpace(enteredName)) return;

            Exercise exercise = new()
            {
                Name = enteredName
            };

            await _exerciseData.CreateExerciseAsync(exercise);

            LoadExerciseList();
        }

        [RelayCommand]
        private async Task DeleteExerciseAsync(Exercise exercise)
        {
            if (exercise is null) return;

            await _exerciseData.DeleteExerciseAsync(exercise);

            LoadExerciseList();
        }

        private async Task UpdateExerciseAsync(Exercise exercise)
        {
            if (exercise is null) return;

            await _exerciseData.UpdateExerciseAsync(exercise);

            LoadExerciseList();
        }

        [RelayCommand]
        private async Task AddExerciseGroupToExercise(Exercise exercise)
        {
            if (exercise is null) return;

            await _exerciseData.AddExerciseGroupToExercise(exercise);

            LoadExerciseList();
        }

        [RelayCommand]
        private async Task RemoveExerciseGroupFromExercise(Exercise exercise)
        {
            if (exercise is null) return;

            await _exerciseData.RemoveExerciseGroupFromExercise(exercise);

            LoadExerciseList();
        }

        [RelayCommand]
        private async Task ResetExerciseList()
        {
            bool userClickedDelete = await Shell.Current.DisplayAlert("Reset All Exercises", "This will delete all added Exercises and restore only the default Exercises.\nThis can not be reversed. Are you sure you want to delete all Exercises?", "Delete", "Cancel");

            if (!userClickedDelete) return;

            await _exerciseData.ResetExerciseListToDefault();

            LoadExerciseList();
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void ShowDefaultExerciseList()
        {
            FilteredDefaultExerciseList = new(DefaultExerciseList);

            DisplayDefaultExerciseList = true;
        }

        [RelayCommand]
        private void HideDefaultExerciseList()
        {
            DisplayDefaultExerciseList = false;
        }

        public async Task RestoreExercise(Exercise exercise)
        {
            if (exercise is null) return;

            await _exerciseData.CreateExerciseAsync(exercise);

            LoadExerciseList();

            HideDefaultExerciseList();
        }

        [RelayCommand]
        private async Task GoToExerciseDetailsPage(Exercise exercise)
        {
            if (exercise is null) return;

            ExerciseDetailsPackage exercisePackage = new()
            {
                Exercise = exercise,
                IsComingFromWorkoutPage = false,
            };

            var navigationParameter = new Dictionary<string, object>
            {
                ["ExerciseDetailsPackage"] = exercisePackage
            };

            await Shell.Current.GoToAsync($"{nameof(ExerciseDetailsPage)}?Id={exercise.Id}", navigationParameter);
        }
    }
}
