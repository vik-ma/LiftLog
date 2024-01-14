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

        [RelayCommand]
        private async Task GoToWorkoutList()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutListPage)}");
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

        private async Task CreateWorkoutAsync(Workout workout)
        {
            if (workout is null) return;

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<Workout>(workout);
            });
        }

        [RelayCommand]
        private async Task GoToNewWorkout()
        {
            string currentYmdDateString = DateTimeHelper.GetCurrentFormattedYmdDate();

            Workout newWorkout = new()
            {
                Date = currentYmdDateString,
            };

            await CreateWorkoutAsync(newWorkout);

            var navigationParameter = new Dictionary<string, object>
            {
                ["Workout"] = newWorkout
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutPage)}?Id={newWorkout.Id}", navigationParameter);
        }
    }
}
