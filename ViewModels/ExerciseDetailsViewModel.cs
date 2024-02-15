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

        [ObservableProperty]
        private ObservableCollection<Set> setList = [];

        public async Task LoadSetsFromExerciseIdAsync()
        {
            if (Exercise is null) return;

            SetList.Clear();

            Expression<Func<Set, bool>> predicate = entity => entity.ExerciseId == Exercise.Id && entity.IsCompleted;

            IEnumerable<Set> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<Set>(predicate);

                foreach (var item in filteredList)
                {
                    SetList.Add(item);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Sets.", "OK");
                return;
            }
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
