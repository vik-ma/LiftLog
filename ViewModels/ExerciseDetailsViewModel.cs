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

        [ObservableProperty]
        private Set maxWeightSet;

        [ObservableProperty]
        private Set maxDistanceSet;

        public async Task LoadSetsFromExerciseIdAsync()
        {
            if (Exercise is null) return;

            SetList.Clear();

            Expression<Func<Set, bool>> predicate = entity => entity.ExerciseId == Exercise.Id && entity.IsCompleted;

            IEnumerable<Set> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<Set>(predicate);

                foreach (var set in filteredList)
                {
                    SetList.Add(set);

                    UpdateMaxSetForExericise(set);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Sets.", "OK");
                return;
            }
        }
        public void UpdateMaxSetForExericise(Set set)
        {
            if (set is null) return;

            if (set.IsTrackingWeight && set.Weight > MaxWeightSet.Weight) MaxWeightSet = set;

            if (set.IsTrackingDistance && set.Distance > MaxDistanceSet.Distance) MaxDistanceSet = set;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
