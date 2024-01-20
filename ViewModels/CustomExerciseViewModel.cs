namespace LocalLiftLog.ViewModels
{
    public partial class CustomExerciseViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;
        //private readonly ExerciseDataManager _exerciseData;

        public CustomExerciseViewModel(DatabaseContext context)
        {
            _context = context;
            //_exerciseData = exerciseData;
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
            await ExecuteAsync(async () =>
            {
                Exercise exercise = new();
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

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
