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
    }
}
