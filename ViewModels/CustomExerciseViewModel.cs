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

            string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Exercise Group", "Enter Exercise Group Int To Add", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

            if (!validInput)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Input Value.", "OK");
                return;
            }

            bool success = exercise.AddExerciseGroup(enteredNumberInt);

            if (!success)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Exercise Group Int.", "OK");
                return;
            }

            await UpdateExerciseAsync(exercise);
        }

        [RelayCommand]
        private async Task RemoveExerciseGroupToExercise(Exercise exercise)
        {
            if (exercise is null) return;

            string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Exercise Group", "Enter Exercise Group Int To Remove", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

            if (!validInput)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Input Value.", "OK");
                return;
            }

            bool success = exercise.RemoveExerciseGroup(enteredNumberInt);

            if (!success)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Exercise Group Int.", "OK");
                return;
            }

            await UpdateExerciseAsync(exercise);
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
