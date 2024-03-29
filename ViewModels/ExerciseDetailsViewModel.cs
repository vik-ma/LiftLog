﻿namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(ExerciseDetailsPackage), nameof(ExerciseDetailsPackage))]
    public partial class ExerciseDetailsViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private ExerciseDetailsPackage exerciseDetailsPackage;
        public ExerciseDetailsViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private List<Set> setList = [];

        [ObservableProperty]
        private ObservableCollection<SetGroup> setGroupList = [];

        [ObservableProperty]
        private Set maxWeightSet;

        [ObservableProperty]
        private Set maxDistanceSet;

        public async Task LoadSetsFromExerciseIdAsync()
        {
            if (ExerciseDetailsPackage is null || ExerciseDetailsPackage.Exercise is null) return;

            SetList.Clear();

            Expression<Func<Set, bool>> predicate = entity => entity.ExerciseId == ExerciseDetailsPackage.Exercise.Id && entity.IsCompleted;

            IEnumerable<Set> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<Set>(predicate);

                foreach (var set in filteredList)
                {
                    SetList.Add(set);

                    UpdateMaxSetForExercise(set);
                }

                // Create a dictionary where every key is the yyyy-MM-dd date
                // and every value is a list, ordered by TimeCompleted
                var setDict = SetList.OrderBy(x => DateTime.Parse(x.TimeCompleted).TimeOfDay)
                    .GroupBy(o => DateTime.Parse(o.TimeCompleted).ToString("yyyy-MM-dd"))
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in setDict)
                {
                    SetGroupList.Add(new SetGroup(item.Key, item.Value));
                }

                SetGroupList = new(SetGroupList.OrderByDescending(x => x.Date));
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

        [RelayCommand]
        private async Task GoToWorkoutPage()
        {
            if (ExerciseDetailsPackage is null || !ExerciseDetailsPackage.IsComingFromWorkoutPage || ExerciseDetailsPackage.Workout is null) 
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["Workout"] = ExerciseDetailsPackage.Workout
            };

            var currentPage = Shell.Current.CurrentPage;

            await Shell.Current.GoToAsync($"{nameof(WorkoutPage)}?Id={ExerciseDetailsPackage.Workout.Id}", navigationParameter);

            Shell.Current.Navigation.RemovePage(currentPage);
        }

        [RelayCommand]
        private async Task GoToWorkoutOperatingSetPage()
        {
            if (ExerciseDetailsPackage is null || !ExerciseDetailsPackage.IsComingFromWorkoutPage || ExerciseDetailsPackage.Workout is null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["Workout"] = ExerciseDetailsPackage.Workout
            };

            var currentPage = Shell.Current.CurrentPage;

            await Shell.Current.GoToAsync($"{nameof(WorkoutOperatingSetPage)}?Id={ExerciseDetailsPackage.Workout.Id}", navigationParameter);

            Shell.Current.Navigation.RemovePage(currentPage);
        }
    }
}
