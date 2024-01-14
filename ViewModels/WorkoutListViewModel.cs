namespace LocalLiftLog.ViewModels
{
    public partial class WorkoutListViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public WorkoutListViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<Workout> workoutList = new();

        public async Task LoadWorkoutsAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutList.Clear();

                var workouts = await _context.GetAllAsync<Workout>();

                if (workouts is not null && workouts.Any())
                {
                    workouts ??= new ObservableCollection<Workout>();

                    foreach (var workout in workouts)
                    {
                        WorkoutList.Add(workout);
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
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task DeleteWorkoutAsync(Workout workout)
        {
            if (workout is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<Workout>(workout))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout.", "OK");
                }
            });

            await LoadWorkoutsAsync();
        }

        [RelayCommand]
        private async Task GoToWorkout(Workout workout)
        {
            if (workout is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["Workout"] = workout
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutPage)}?Id={workout.Id}", navigationParameter);
        }
    }
}
