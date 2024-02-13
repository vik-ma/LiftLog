namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(Exercise), nameof(Exercise))]
    public partial class ExerciseDetailsViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private Exercise exercise;

        public ExerciseDetailsViewModel(DatabaseContext context)
        {
            _context = context;
        }
    }
}
