namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(Workout), nameof(Workout))]
    public partial class WorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        [ObservableProperty]
        private Workout workout;

        public WorkoutViewModel(DatabaseContext context, UserPreferencesViewModel userSettings)
        {
            _context = context;
            userSettingsViewModel = userSettings;
        }

        [ObservableProperty]
        private DateTime selectedDateTime;

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

        public void LoadSelectedDateTime()
        {
            if (Workout is null) return;

            SelectedDateTime = DateTimeHelper.FormatDateTimeYmdStringToDateTime(Workout.Date);
        }

        [RelayCommand]
        private async Task UpdateWorkoutAsync()
        {
            if (Workout is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<Workout>(Workout))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task SetWorkoutName()
        {
            if (Workout is null) return;

            string enteredName = await Shell.Current.DisplayPromptAsync("Workout Name", "Enter a name for the Workout", "OK", "Cancel");

            if (enteredName is null || string.IsNullOrWhiteSpace(enteredName)) return;

            Workout.Name = enteredName;

            await UpdateWorkoutAsync();
        }

        [RelayCommand]
        private async Task ResetWorkoutName()
        {
            if (Workout is null || Workout.Name is null) return;

            bool userClickedReset = await Shell.Current.DisplayAlert("Reset Name", "Do you really want to reset Workout Name?", "Reset", "Cancel");

            if (!userClickedReset) return;

            Workout.Name = null;

            await UpdateWorkoutAsync();
        }

        public async Task UpdateWorkoutDate(DateTime selectedDate)
        {
            if (Workout is null) return;

            string ymdDateString = DateTimeHelper.FormatDateTimeToYmdString(selectedDate);
        
            Workout.Date = ymdDateString;

            await UpdateWorkoutAsync();
        }
    }
}
