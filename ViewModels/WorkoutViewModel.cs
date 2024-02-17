using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.System;

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

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> filteredWorkoutTemplateList = new();

        [ObservableProperty]
        private WorkoutTemplate operatingWorkoutTemplate = new();

        private WorkoutTemplateListPopupPage Popup;

        [ObservableProperty]
        private string popupTitle = "Load Workout";

        private readonly List<int> SetListIdOrder = new();

        [ObservableProperty]
        private ObservableCollection<SetExercisePackage> setList = new();

        [ObservableProperty]
        private bool workoutTemplateContainsInvalidExercise = false;

        [ObservableProperty]
        private bool savedSetsContainsDeletedExercise = false;

        private SetExercisePackage setBeingDragged;

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

        public async Task LoadWorkoutTemplatesAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutTemplateList.Clear();

                var workoutTemplates = await _context.GetAllAsync<WorkoutTemplate>();

                if (workoutTemplates is not null && workoutTemplates.Any())
                {
                    workoutTemplates ??= new ObservableCollection<WorkoutTemplate>();

                    foreach (var workout in workoutTemplates)
                    {
                        WorkoutTemplateList.Add(workout);
                    }
                }

                FilteredWorkoutTemplateList = new(WorkoutTemplateList);
            });
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

            OnPropertyChanged(nameof(Workout));
        }

        public async Task UpdateWorkoutDate(DateTime selectedDate)
        {
            if (Workout is null) return;

            string ymdDateString = DateTimeHelper.FormatDateTimeToYmdString(selectedDate);
        
            Workout.Date = ymdDateString;

            await UpdateWorkoutAsync();
        }

        public async Task LoadSetList()
        {
            if (Workout is null) return;

            await LoadOperatingWorkoutTemplateAsync();

            // Load existing Sets for Workout to SetList
            await LoadSavedSetsAsync();

            if (!Workout.IsWorkoutLoaded)
            {
                // Create new Sets from WorkoutTemplate and add to SetList
                await LoadSetListFromWorkoutTemplateIdAsync();
            }
        }

        private async Task LoadSavedSetsAsync()
        {
            if (Workout is null) return;

            LoadSetListIdOrder(Workout.SetListIdOrder);

            SetList.Clear();

            SavedSetsContainsDeletedExercise = false;

            List<SetExercisePackage> setTemplateExercisePackageList = new();

            Expression<Func<Set, bool>> predicateSet = entity => entity.WorkoutId == Workout.Id;

            try
            {
                var filteredSetList = await _context.GetFilteredAsync<Set>(predicateSet);

                foreach (var set in filteredSetList)
                {
                    Exercise exercise = await _context.GetItemByKeyAsync<Exercise>(set.ExerciseId);

                    if (exercise is null) SavedSetsContainsDeletedExercise = true;

                    SetExercisePackage setTemplateExercisePackage = new()
                    {
                        Exercise = exercise ?? new() { Name = "Deleted Exercise" },
                        Set = set,
                    };

                    setTemplateExercisePackageList.Add(setTemplateExercisePackage);
                }

                if (setTemplateExercisePackageList.Any())
                {
                    // Sort the SetList by Workouts SetListIdOrder
                    SetList = new ObservableCollection<SetExercisePackage>(setTemplateExercisePackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.Set.Id)));
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Templates.", "OK");
                return;
            }
        }

        private async Task LoadOperatingWorkoutTemplateAsync()
        {
            if (Workout is null) return;

            if (Workout.WorkoutTemplateId == 0) return;

            WorkoutTemplate workoutTemplate = null;

            await ExecuteAsync(async () =>
            {
                workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(Workout.WorkoutTemplateId);
            });

            if (workoutTemplate is null)
            {
                // Reset WorkoutTemplateId property if no WorkoutTemplate with that Id was found
                await ResetWorkoutTemplateId();
                await Shell.Current.DisplayAlert("Invalid Workout Template", "Workout contained a reference to a Workout Template that no longer exists and has been removed from Workout.", "OK");
                return;
            }

            OperatingWorkoutTemplate = workoutTemplate;
        }

        [RelayCommand]
        private async Task ResetWorkoutTemplateId()
        {
            if (Workout is null) return;

            Workout.WorkoutTemplateId = 0;

            await UpdateWorkoutAsync();

            await DeleteIncompleteSets();
        }

        private async Task DeleteIncompleteSets()
        {
            if (SetList is null) return;

            IEnumerable<SetExercisePackage> incompletedSets = SetList.Where(item => !item.Set.IsCompleted);

            if (incompletedSets.Any())
            {
                foreach (var set in incompletedSets)
                {
                    await DeleteSetAsync(set.Set);
                }

                SetList = new(SetList.Where(item => item.Set.IsCompleted));

                await GenerateSetListOrderString();
            }
        }

        [RelayCommand]
        private async Task ShowDeleteIncompleteSetsPrompt()
        {
            if (SetList.Any()) 
            {
                bool userClickedRemove = await Shell.Current.DisplayAlert("Remove Incomplete Sets", "Remove all incomplete sets from workout?", "Remove", "Cancel");

                if (!userClickedRemove) return;
            }

            await DeleteIncompleteSets();
        }

        private void LoadSetListIdOrder(string setListIdOrder)
        {
            if (string.IsNullOrEmpty(setListIdOrder)) return;

            string[] setList = setListIdOrder.Split(",");

            foreach (string s in setList)
            {
                if (int.TryParse(s, out int setId))
                {
                    SetListIdOrder.Add(setId);
                }
            }
        }

        public async Task LoadSetListFromWorkoutTemplateIdAsync()
        {
            if (OperatingWorkoutTemplate is null || OperatingWorkoutTemplate.Id == 0) return;

            LoadSetListIdOrder(OperatingWorkoutTemplate.SetListOrder);

            WorkoutTemplateContainsInvalidExercise = false;

            // Retain completed Sets from existing SetList
            SetList = new(SetList.Where(item => item.Set.IsCompleted));

            List<SetExercisePackage> setExercisePackageList = new();

            Expression<Func<Set, bool>> predicateSet = entity => entity.WorkoutTemplateId == OperatingWorkoutTemplate.Id && entity.IsTemplate == true;

            try
            {
                var filteredSetList = await _context.GetFilteredAsync<Set>(predicateSet);

                foreach (var set in filteredSetList)
                {
                    Exercise exercise = await _context.GetItemByKeyAsync<Exercise>(set.ExerciseId);

                    // Don't add Set if Exercise Id does not exist 
                    if (exercise is null)
                    {
                        WorkoutTemplateContainsInvalidExercise = true;
                        continue;
                    }

                    Set newSet = set.Clone();

                    newSet.IsTemplate = false;
                    newSet.WorkoutId = Workout.Id;

                    await CreateSetAsync(newSet);

                    SetExercisePackage setTemplateExercisePackage = new()
                    {
                        Exercise = exercise,
                        Set = newSet,
                    };

                    setExercisePackageList.Add(setTemplateExercisePackage);
                }

                // Set Workout as loaded, as to not create new Sets next time Workout is opened
                Workout.IsWorkoutLoaded = true;

                if (setExercisePackageList.Any())
                {
                    // Sort setExercisePackageList by its SetListIdOrder
                    List<SetExercisePackage> sortedSetExercisePackageList = new(setExercisePackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.Set.Id)));

                    // Add setExercisePackageList to the end of SetList
                    foreach (var set in sortedSetExercisePackageList)
                    {
                        SetList.Add(set);
                    }
                }

                // GenerateSetListOrderString also saves Workout
                await GenerateSetListOrderString();

                if (WorkoutTemplateContainsInvalidExercise)
                {
                    await Shell.Current.DisplayAlert("Invalid Exercise", "Workout Template contains a Set with an Exercise that no longer exists and was not added to Workout.", "OK");
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Workout Template.", "OK");
                return;
            }
        }

        private async Task CreateSetAsync(Set set)
        {
            if (set is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.AddItemAsync<Set>(set))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to save Set.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task DeleteSetAsync(Set set)
        {
            if (set is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<Set>(set))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task DeleteSetFromSetList(SetExercisePackage setExercisePackage)
        {
            if (setExercisePackage is null || setExercisePackage.Set is null) return;

            bool userClickedDelete = await Shell.Current.DisplayAlert("Delete Set", "Are you sure you want to delete set?", "Delete", "Cancel");

            if (!userClickedDelete) return;

            await DeleteSetAsync(setExercisePackage.Set);

            SetList.Remove(setExercisePackage);

            await GenerateSetListOrderString();
        }

        public async Task UpdateOperatingWorkoutTemplate(int workoutTemplateId)
        {
            if (workoutTemplateId < 1) return;

            Workout.WorkoutTemplateId = workoutTemplateId;

            await UpdateWorkoutAsync();

            await DeleteIncompleteSets();

            await LoadOperatingWorkoutTemplateAsync();

            await LoadSetListFromWorkoutTemplateIdAsync();
        }

        [RelayCommand]
        private async Task ShowWorkoutTemplateListPopup()
        {
            if (Workout is null) return;

            IEnumerable<SetExercisePackage> incompletedSets = SetList.Where(item => !item.Set.IsCompleted);

            if (incompletedSets.Any()) 
            {
                bool userClickedReload = await Shell.Current.DisplayAlert("Load Workout Template", "Delete all incomplete sets and load sets from a Workout Template?", "Load", "Cancel");

                if (!userClickedReload) return;
            }

            await LoadWorkoutTemplatesAsync();

            WorkoutTemplateListPopupPageHandler handler = new()
            {
                ViewModelType = "Workout",
                WorkoutViewModel = this,
            };
            Popup = new WorkoutTemplateListPopupPage(handler);
            await Shell.Current.ShowPopupAsync(Popup);
        }

        [RelayCommand]
        public void ClosePopup()
        {
            if (Popup is null) return;

            Popup.Close();
        }

        [RelayCommand]
        private async Task GoToWorkoutTemplateDetails()
        {
            ClosePopup();

            WorkoutTemplate workoutTemplate = new();

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutTemplate"] = workoutTemplate
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?Id={workoutTemplate.Id}", navigationParameter);
        }

        public async Task GenerateSetListOrderString()
        {
            if (Workout is null) return;

            IEnumerable<string> setIdList = SetList.Select(set => set.Set.Id.ToString());

            Workout.SetListIdOrder = string.Join(",", setIdList);

            await UpdateWorkoutAsync();
        }

        [RelayCommand]
        private async Task MoveSetUp(Set set)
        {
            if (set is null) return;

            // Get SetList index of current Set
            int setIndex = SetList.ToList().FindIndex(item => item.Set.Equals(set));

            // Do nothing if item is already first in list
            if (setIndex < 1) return;

            // Swap current Set with Set at the index before
            (SetList[setIndex - 1], SetList[setIndex]) = (SetList[setIndex], SetList[setIndex - 1]);

            await GenerateSetListOrderString();
        }

        [RelayCommand]
        private async Task MoveSetDown(Set set)
        {
            if (set is null) return;

            // Get SetList index of current Set
            int setIndex = SetList.ToList().FindIndex(item => item.Set.Equals(set));

            // Do nothing if item is already last in list
            if (setIndex > SetList.Count - 2) return;

            // Swap current Set with Set at the index after
            (SetList[setIndex + 1], SetList[setIndex]) = (SetList[setIndex], SetList[setIndex + 1]);

            await GenerateSetListOrderString();
        }

        private async Task UpdateSetAsync(Set set)
        {
            if (set is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<Set>(set))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to update Set.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task ToggleSetCompleted(Set set)
        {
            if (set is null) return;

            set.IsCompleted = !set.IsCompleted;

            if (set.IsCompleted) set.TimeCompleted = DateTimeHelper.GetCurrentFormattedDateTime();
            else set.TimeCompleted = null;

            await UpdateSetAsync(set);
        }

        [RelayCommand]
        private async Task SetTimeCompletedProperty(Set set)
        {
            if (set is null) return;

            string enteredDateTime = await Shell.Current.DisplayPromptAsync("Enter DateTime String", "", "OK", "Cancel");

            if (enteredDateTime == null) return;

            if (!DateTimeHelper.ValidateDateTimeString(enteredDateTime)) 
            {
                await Shell.Current.DisplayAlert("Error", "Invalid DateTime.", "OK");
                return;
            }

            set.IsCompleted = true;
            set.TimeCompleted = enteredDateTime;

            await UpdateSetAsync(set);
        }

        [RelayCommand]
        public void SetDragged(SetExercisePackage package)
        {
            package.IsBeingDragged = true;
            setBeingDragged = package;
        }

        [RelayCommand]
        public static void SetDragLeave(SetExercisePackage package)
        {
            package.IsBeingDraggedOver = false;
        }

        [RelayCommand]
        public void SetDraggedOver(SetExercisePackage package)
        {
            if (package == setBeingDragged)
            {
                package.IsBeingDragged = false;
            }
            package.IsBeingDraggedOver = package != setBeingDragged;
        }

        [RelayCommand]
        public async Task SetDropped(SetExercisePackage package)
        {
            try
            {
                var setToMove = setBeingDragged;
                var setToInsertBefore = package;

                if (setToMove == null || setToInsertBefore == null || setToMove == setToInsertBefore)
                    return;

                int insertAtIndex = SetList.IndexOf(setToInsertBefore);

                if (insertAtIndex >= 0 && insertAtIndex < SetList.Count)
                {
                    SetList.Remove(setToMove);
                    SetList.Insert(insertAtIndex, setToMove);
                    setToMove.IsBeingDragged = false;
                    setToInsertBefore.IsBeingDraggedOver = false;
                }

                await GenerateSetListOrderString();
            }
            catch
            {

            }
        }
    }
}
