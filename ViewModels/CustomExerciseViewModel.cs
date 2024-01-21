namespace LocalLiftLog.ViewModels
{
    public partial class CustomExerciseViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        private readonly ExerciseDataManager _exerciseData;

        [ObservableProperty]
        private ObservableCollection<Exercise> exerciseList = new();

        public CustomExerciseViewModel(DatabaseContext context, ExerciseDataManager exerciseData)
        {
            _context = context;
            _exerciseData = exerciseData;
        }

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
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
