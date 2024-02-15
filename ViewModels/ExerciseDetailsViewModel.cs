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

                    UpdateMaxSetForExercise(set);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Sets.", "OK");
                return;
            }
        }
        public void UpdateMaxSetForExercise(Set set)
        {
            if (set is null) return;

            if (set.IsTrackingWeight) 
            {
                if (MaxWeightSet is null) 
                    MaxWeightSet = set;
                else
                    if (set.Weight > MaxWeightSet.Weight) MaxWeightSet = set;
            }

            if (set.IsTrackingDistance)
            {
                if (MaxDistanceSet is null)
                    MaxDistanceSet = set;
                else
                    if (set.Distance > MaxDistanceSet.Distance) MaxDistanceSet = set;
            }
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
