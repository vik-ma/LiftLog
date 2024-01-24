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

        public async Task LoadWorkoutTemplateAsync()
        {
            if (Workout is null) return;

            if (Workout.WorkoutTemplateId == 0) return;

            WorkoutTemplate workoutTemplate = null;

            await ExecuteAsync(async () =>
            {
                workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(Workout.WorkoutTemplateId);
            });

            if (workoutTemplate is null) return;

            OperatingWorkoutTemplate = workoutTemplate;

            await LoadSetListFromWorkoutTemplateIdAsync();
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

            List<SetTemplateExercisePackage> setTemplateExercisePackageList = new();

            Expression<Func<SetTemplate, bool>> predicateSetTemplate = entity => entity.WorkoutTemplateId == OperatingWorkoutTemplate.Id;

            try
            {
                var filteredSetTemplateList = await _context.GetFilteredAsync<SetTemplate>(predicateSetTemplate);

                foreach (var item in filteredSetTemplateList)
                {
                    Exercise exercise = await _context.GetItemByKeyAsync<Exercise>(item.ExerciseId);

                    SetTemplateExercisePackage setTemplateExercisePackage = new()
                    {
                        SetTemplate = item,
                        Exercise = exercise ?? new() { Name = "Invalid Exercise" } 
                    };

                    setTemplateExercisePackageList.Add(setTemplateExercisePackage);
                }

                if (setTemplateExercisePackageList.Any())
                {
                    // Sort the SetList by its SetListIdOrder
                    SetList = new ObservableCollection<SetTemplateExercisePackage>(setTemplateExercisePackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.SetTemplate.Id)));
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Templates.", "OK");
                return;
            }
        }

        public async Task UpdateOperatingWorkoutTemplate(int workoutTemplateId)
        {
            if (workoutTemplateId < 1) return;

            Workout.WorkoutTemplateId = workoutTemplateId;

            await UpdateWorkoutAsync();

            await LoadWorkoutTemplateAsync();
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
