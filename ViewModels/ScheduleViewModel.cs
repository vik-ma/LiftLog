namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutRoutine), nameof(WorkoutRoutine))]
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutRoutine workoutRoutine;

        public ScheduleViewModel(DatabaseContext context)
        {
            _context = context;
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
    }
}
