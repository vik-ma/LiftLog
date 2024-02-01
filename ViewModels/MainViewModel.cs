namespace LocalLiftLog.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        private readonly ExerciseDataManager _exerciseData;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        public MainViewModel(DatabaseContext context, ExerciseDataManager exerciseData, UserPreferencesViewModel userSettings)
        {
            _context = context;
            _exerciseData = exerciseData;
            userSettingsViewModel = userSettings;
        }

        public async Task InitializeUserPreferences()
        {
            await UserSettingsViewModel.InitializeUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task GoToWorkoutRoutineList()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutRoutineListPage)}");
        }

        [RelayCommand]
        private async Task GoToWorkoutTemplate()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutTemplateListPage)}");
        }

        [RelayCommand]
        private async Task GoToSetList()
        {
            await Shell.Current.GoToAsync($"{nameof(SetListPage)}");
        }

        [RelayCommand]
        private async Task GoToCompletedWorkout()
        {
            await Shell.Current.GoToAsync($"{nameof(CompletedWorkoutListPage)}");
        }

        [RelayCommand]
        private async Task GoToTimePeriod()
        {
            await Shell.Current.GoToAsync($"{nameof(TimePeriodListPage)}");
        }

        [RelayCommand]
        private async Task GoToCustomExercise()
        {
            await Shell.Current.GoToAsync($"{nameof(CustomExerciseListPage)}");
        }

        [RelayCommand]
        private async Task GoToExerciseList()
        {
            await Shell.Current.GoToAsync($"{nameof(ExerciseListPage)}");
        }

        [RelayCommand]
        private async Task GoToSelectWorkout()
        {
            await Shell.Current.GoToAsync($"{nameof(SelectWorkoutPage)}");
        }

        [RelayCommand]
        private async Task GoToUserPreferences()
        {
            await Shell.Current.GoToAsync($"{nameof(UserPreferencesPage)}");
        }

        [RelayCommand]
        private async Task GoToUserWeight()
        {
            await Shell.Current.GoToAsync($"{nameof(UserWeightPage)}");
        }

        [RelayCommand]
        private async Task GoToNewWorkout()
        {
            await Shell.Current.GoToAsync($"{nameof(NewWorkoutPage)}");
        }

        [RelayCommand]
        private async Task GoToWorkoutList()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutListPage)}");
        }
    }
}
