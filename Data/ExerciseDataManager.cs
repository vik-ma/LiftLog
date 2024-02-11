namespace LocalLiftLog.Data
{
    public class ExerciseDataManager
    {
        private readonly DatabaseContext _context;

        public readonly List<Exercise> ExerciseList = new();
        public ExerciseDataManager(DatabaseContext context) 
        {
            _context = context;
            LoadExerciseList();
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

        private async Task CreateDefaultExerciseList()
        {
            List<Exercise> defaultExerciseList = DefaultExercises.DefaultExerciseList; 

            foreach (var exercise in defaultExerciseList)
            {
                await CreateExerciseAsync(exercise);
            }
        }

        public async Task CreateExerciseAsync(Exercise exercise)
        {
            if (exercise is null) return;

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<Exercise>(exercise);
                ExerciseList.Add(exercise);
            });
        }

        private async void LoadExerciseList()
        {
            await LoadExercisesAsync();

            if (!ExerciseList.Any())
            {
                // Add default Exercises if ExerciseList is empty
                await CreateDefaultExerciseList();
            }
        }

        private async Task LoadExercisesAsync()
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

        public async Task UpdateExerciseAsync(Exercise exercise)
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

        public async Task DeleteExerciseAsync(Exercise exercise)
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

        public async Task ResetExerciseListToDefault()
        {
            await ExecuteAsync(async () =>
            {
                foreach (var exercise in ExerciseList)
                {
                    await _context.DeleteItemAsync<Exercise>(exercise);
                }
            });

            await CreateDefaultExerciseList();

            await LoadExercisesAsync();
        }

        public ObservableCollection<Exercise> FilterExerciseListByExerciseGroups(HashSet<int> groupSet)
        {
            if (groupSet is null || groupSet.Count == 0) return new ObservableCollection<Exercise>(ExerciseList);

            return new ObservableCollection<Exercise>(ExerciseList.Where(item => groupSet.Any(group => item.GetExerciseGroupHashSet().Contains(group))));
        }

        public async Task AddExerciseGroupToExercise(Exercise exercise)
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

        public async Task RemoveExerciseGroupFromExercise(Exercise exercise)
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
    }
}
