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

        private readonly List<int> SetListIdOrder = new();

        [ObservableProperty]
        private ObservableCollection<SetTemplateExercisePackage> setList = new();

        [ObservableProperty]
        private bool workoutTemplateContainsInvalidExercise = false;

        [ObservableProperty]
        private bool savedSetsContainsDeletedExercise = false;

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

        [RelayCommand]
        private async Task SetWorkoutName()
        {
            if (Workout is null) return;

            string enteredName = await Shell.Current.DisplayPromptAsync("Workout Name", "Enter a name for the Workout", "OK", "Cancel");

            if (enteredName is null || string.IsNullOrWhiteSpace(enteredName)) return;

            Workout.Name = enteredName;

            await UpdateWorkoutAsync();
        }

        [RelayCommand]
        private async Task ResetWorkoutName()
        {
            if (Workout is null || Workout.Name is null) return;

            bool userClickedReset = await Shell.Current.DisplayAlert("Reset Name", "Do you really want to reset Workout Name?", "Reset", "Cancel");

            if (!userClickedReset) return;

            Workout.Name = null;

            await UpdateWorkoutAsync();
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

            if (Workout.IsWorkoutLoaded)
            {
                // Load existing Sets for Workout to SetList
                await LoadSavedSetsAsync();
            }
            else
            {
                // Create new Sets from WorkoutTemplate and add to SetList
                await LoadOperatingWorkoutTemplateAsync();
            }
        }

        private async Task LoadSavedSetsAsync()
        {
            if (Workout is null) return;

            LoadSetListIdOrder(Workout.SetListIdOrder);

            SetList.Clear();

            SavedSetsContainsDeletedExercise = false;

            List<SetTemplateExercisePackage> setTemplateExercisePackageList = new();

            Expression<Func<Set, bool>> predicateSet = entity => entity.WorkoutId == Workout.Id;

            try
            {
                var filteredSetList = await _context.GetFilteredAsync<Set>(predicateSet);

                foreach (var set in filteredSetList)
                {
                    Exercise exercise = await _context.GetItemByKeyAsync<Exercise>(set.ExerciseId);

                    if (exercise is null) SavedSetsContainsDeletedExercise = true;

                    SetTemplate setTemplate = await _context.GetItemByKeyAsync<SetTemplate>(set.SetTemplateId);

                    SetTemplateExercisePackage setTemplateExercisePackage = new()
                    {
                        SetTemplate = setTemplate ?? new(),
                        Exercise = exercise ?? new() { Name = "Deleted Exercise" },
                        Set = set,
                    };

                    setTemplateExercisePackageList.Add(setTemplateExercisePackage);
                }

                if (setTemplateExercisePackageList.Any())
                {
                    // Sort the SetList by Workouts SetListIdOrder
                    SetList = new ObservableCollection<SetTemplateExercisePackage>(setTemplateExercisePackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.Set.Id)));
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

            await LoadSetListFromWorkoutTemplateIdAsync();
        }

        [RelayCommand]
        private async Task ResetWorkoutTemplateId()
        {
            if (Workout is null) return;

            Workout.WorkoutTemplateId = 0;

            if (SetList.Any())
            {
                // TODO: ADD PROMPT TO ALSO REMOVE INCOMPLETE SETS FROM SETLIST 
                SetList = new();
                Workout.SetListIdOrder = null;
            }

            await UpdateWorkoutAsync();
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
            if (OperatingWorkoutTemplate is null) return;

            LoadSetListIdOrder(OperatingWorkoutTemplate.SetListOrder);

            SetList.Clear();

            WorkoutTemplateContainsInvalidExercise = false;

            List<SetTemplateExercisePackage> setTemplateExercisePackageList = new();

            Expression<Func<SetTemplate, bool>> predicateSetTemplate = entity => entity.WorkoutTemplateId == OperatingWorkoutTemplate.Id;

            try
            {
                var filteredSetTemplateList = await _context.GetFilteredAsync<SetTemplate>(predicateSetTemplate);

                foreach (var setTemplate in filteredSetTemplateList)
                {
                    Exercise exercise = await _context.GetItemByKeyAsync<Exercise>(setTemplate.ExerciseId);

                    // Don't add Set if Exercise Id does not exist 
                    if (exercise is null)
                    {
                        WorkoutTemplateContainsInvalidExercise = true;
                        continue;
                    }

                    Set newSet = new()
                    {
                        WorkoutId = Workout.Id,
                        ExerciseId = exercise.Id,
                        SetTemplateId = setTemplate.Id,
                    };

                    await CreateSetAsync(newSet);

                    SetTemplateExercisePackage setTemplateExercisePackage = new()
                    {
                        SetTemplate = setTemplate,
                        Exercise = exercise,
                        Set = newSet,
                    };

                    setTemplateExercisePackageList.Add(setTemplateExercisePackage);
                }

                // Set Workout as loaded, as to not create new Sets next time Workout is opened
                Workout.IsWorkoutLoaded = true;
                await UpdateWorkoutAsync();

                if (setTemplateExercisePackageList.Any())
                {
                    // Sort the SetList by its SetListIdOrder
                    SetList = new ObservableCollection<SetTemplateExercisePackage>(setTemplateExercisePackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.SetTemplate.Id)));
                }

                if (WorkoutTemplateContainsInvalidExercise)
                {
                    await Shell.Current.DisplayAlert("Invalid Exercise", "Workout Template contains a Set with an Exercise that no longer exists and was not added to Workout.", "OK");
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Templates.", "OK");
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

        public async Task UpdateOperatingWorkoutTemplate(int workoutTemplateId)
        {
            if (workoutTemplateId < 1) return;

            Workout.WorkoutTemplateId = workoutTemplateId;

            await UpdateWorkoutAsync();

            await LoadOperatingWorkoutTemplateAsync();
        }

        [RelayCommand]
        private async Task ShowWorkoutTemplateListPopup()
        {
            if (Workout is null) return;

            await LoadWorkoutTemplatesAsync();

            Popup = new WorkoutTemplateListPopupPage(this);
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

            await UpdateSetAsync(set);
        }
    }
}
