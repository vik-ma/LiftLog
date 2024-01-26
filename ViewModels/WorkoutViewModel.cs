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

            SetList.Clear();

            WorkoutTemplateContainsInvalidExercise = false;

            List<SetTemplateExercisePackage> setTemplateExercisePackageList = new();

            Expression<Func<Set, bool>> predicateSet = entity => entity.WorkoutId == Workout.Id;

            try
            {
                var filteredSetList = await _context.GetFilteredAsync<Set>(predicateSet);

                foreach (var set in filteredSetList)
                {
                    Exercise exercise = await _context.GetItemByKeyAsync<Exercise>(set.ExerciseId);

                    SetTemplate setTemplate = await _context.GetItemByKeyAsync<SetTemplate>(set.SetTemplateId);

                    SetTemplateExercisePackage setTemplateExercisePackage = new()
                    {
                        SetTemplate = setTemplate ?? new(),
                        Exercise = exercise ?? new() { Name = "Invalid Exercise" },
                        Set = set,
                    };

                    setTemplateExercisePackageList.Add(setTemplateExercisePackage);
                }

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
            }

            await UpdateWorkoutAsync();
        }

        private void LoadSetListIdOrder()
        {
            if (OperatingWorkoutTemplate is null) return;

            if (string.IsNullOrEmpty(OperatingWorkoutTemplate.SetListOrder)) return;

            string[] setList = OperatingWorkoutTemplate.SetListOrder.Split(',');

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

            LoadSetListIdOrder();

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
    }
}
