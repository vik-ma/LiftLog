namespace LocalLiftLog.ViewModels
{
    public partial class NewWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        public NewWorkoutViewModel(DatabaseContext context, UserPreferencesViewModel userSettings)
        {
            _context = context;
            userSettingsViewModel = userSettings;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
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
        private async Task CreateWorkoutAsync(Workout workout)
        {
            if (workout is null) return;

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<Workout>(workout);
            });
        }

        [RelayCommand]
        static async Task GoToNewWorkout()
        {
            Workout workout = new();

            var navigationParameter = new Dictionary<string, object>
            {
                ["Workout"] = workout
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutPage)}?Id={workout.Id}", navigationParameter);
        }
    }
}
