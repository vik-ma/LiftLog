namespace LocalLiftLog.ViewModels
{
    public partial class CustomExerciseViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public CustomExerciseViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<Exercise> exerciseList = new();

        public async Task LoadExercisesAsync()
        {
            await ExecuteAsync(async () =>
            {
                ExerciseList.Clear();

                var exercises = await _context.GetAllAsync<Exercise>();

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
        private async Task CreateExerciseAsync()
        {
            string enteredName = await Shell.Current.DisplayPromptAsync("Enter Name", "Enter Exercise Name", "OK", "Cancel");

            if (string.IsNullOrWhiteSpace(enteredName)) return;

            Exercise exercise = new()
            {
                Name = enteredName
            };

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<Exercise>(exercise);
                ExerciseList.Add(exercise);
            });
        }

        [RelayCommand]
        private async Task DeleteExerciseAsync(Exercise exercise)
        {
            if (exercise is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<Exercise>(exercise))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to delete Exercise.", "OK");
                }
            });

            await LoadExercisesAsync();
        }

        private async Task UpdateExerciseAsync(Exercise exercise)
        {
            if (exercise is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<Exercise>(exercise))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to update Exercise.", "OK");
                }
            });

            await LoadExercisesAsync();
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
