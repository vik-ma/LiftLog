namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(SetWorkoutTemplatePackage), nameof(SetWorkoutTemplatePackage))]
    public partial class CreateSetTemplateViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        private readonly ExerciseDataManager _exerciseData;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        [ObservableProperty]
        private SetWorkoutTemplatePackage setWorkoutTemplatePackage;

        [ObservableProperty]
        private Set operatingSet;

        [ObservableProperty]
        private WorkoutTemplate operatingWorkoutTemplate;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string selectedWeightUnit;

        [ObservableProperty]
        private string selectedDistanceUnit;

        public CreateSetTemplateViewModel(DatabaseContext context, ExerciseDataManager exerciseData, UserPreferencesViewModel userSettings)
        {
            _context = context;
            _exerciseData = exerciseData;
            userSettingsViewModel = userSettings;
        }

        [ObservableProperty]
        public List<Exercise> exerciseList = new();

        [ObservableProperty]
        public ObservableCollection<Exercise> filteredExerciseList = new();

        [ObservableProperty]
        private int numNewSets = 1;

        [ObservableProperty]
        private Exercise selectedExercise;

        [ObservableProperty]
        private List<string> validWeightUnitList = new(ConstantsHelper.ValidWeightUnits);

        [ObservableProperty]
        private List<string> validDistanceUnitList = new(ConstantsHelper.ValidDistanceUnits);

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void LoadExerciseList()
        {
            ExerciseList = _exerciseData.ExerciseList;
            FilteredExerciseList = new(ExerciseList);
        }

        public async void InitializeSetWorkoutTemplatePackage()
        {
            OperatingSet = SetWorkoutTemplatePackage.Set;
            OperatingWorkoutTemplate = SetWorkoutTemplatePackage.WorkoutTemplate;
            IsEditing = SetWorkoutTemplatePackage.IsEditing;

            if (IsEditing)
            {
                await LoadExerciseAsync();

                if (SelectedExercise.Id == 0)
                {
                    // Reset ExerciseId if Exercise does not exist
                    OperatingSet.ExerciseId = 0;

                    await Shell.Current.DisplayAlert("Invalid Exercise", "This Set references an Exercise that no longer exist.\nUpdate the Exercise or Set won't appear in Workout.", "OK");
                }
            }
        }

        private async Task LoadExerciseAsync()
        {
            if (OperatingSet is null) return;

            await ExecuteAsync(async () =>
            {
                SelectedExercise = await _context.GetItemByKeyAsync<Exercise>(OperatingSet.ExerciseId) ?? new();
            });
        }

        public void SetDefaultUnitValues()
        {
            if (OperatingSet is null) return;

            // Set SelectedWeightUnit and SelectedDistanceUnit to saved unit in Set if Set is saved
            // Otherwise load default unit based on UserSettings

            if (!string.IsNullOrEmpty(OperatingSet.WeightUnit)) SelectedWeightUnit = OperatingSet.WeightUnit;
            else SelectedWeightUnit = UserSettingsViewModel.UserSettings.IsUsingMetricUnits ? ConstantsHelper.DefaultWeightUnitMetricTrue : ConstantsHelper.DefaultWeightUnitMetricFalse;
            
            if (!string.IsNullOrEmpty(OperatingSet.DistanceUnit)) SelectedDistanceUnit = OperatingSet.DistanceUnit;
            else SelectedDistanceUnit = UserSettingsViewModel.UserSettings.IsUsingMetricUnits ? ConstantsHelper.DefaultDistanceUnitMetricTrue : ConstantsHelper.DefaultDistanceUnitMetricFalse;
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

        [RelayCommand]
        private async Task SaveSetAsync()
        {
            if (OperatingSet is null || OperatingWorkoutTemplate is null) return;

            if (SelectedExercise is null)
            {
                await Shell.Current.DisplayAlert("Error", "No Exercise Selected!", "OK");
                return;
            }

            OperatingSet.IsTemplate = true;
            OperatingSet.WorkoutTemplateId = OperatingWorkoutTemplate.Id;
            OperatingSet.ExerciseId = SelectedExercise.Id;
            OperatingSet.WeightUnit = SelectedWeightUnit;
            OperatingSet.DistanceUnit = SelectedDistanceUnit;

            var (isSetValid, errorMessage) = OperatingSet.ValidateSet();
            if (!isSetValid)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            if (OperatingSet.IsUsingBodyWeightAsWeight)
            {
                // If no Active UserWeight is set, but IsUsingBodyWeightAsWeight has been checked
                if (UserSettingsViewModel.UserSettings.ActiveUserWeightId == 0)
                {
                    bool userUpdatedUserWeight = await ShowUpdateWeightPrompt();

                    // Exit function if user did not enter a valid weight
                    if (!userUpdatedUserWeight) return;
                }

                OperatingSet.DisableBodyWeightTrackingIfNotTrackingWeight();
            }

            if (IsEditing)
            {
                // If editing existing Set
                await UpdateSetAsync();
            }
            else
            {
                // If creating new Set(s)
                int numSets = NumNewSets;

                await CreateNewSetAsync(OperatingSet, numSets);
            }
        }

        private async Task<bool> ShowUpdateWeightPrompt()
        {
            bool userClickedSetBodyWeight = await Shell.Current.DisplayAlert("No Body Weight Set", "A body weight needs to be set in order to add body weight to total weight.\n\nDo you want to set a body weight?", "Set Body Weight", "Cancel");

            if (!userClickedSetBodyWeight) return false;

            double userInputDouble = 0;

            // Keep asking for input until valid input has been entered or user clicks Cancel
            while (userInputDouble == 0)
            {
                string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Body Weight", "Enter your current body weight:\n", "OK", "Cancel");

                // Return false if user clicked Cancel
                if (enteredNumber == null) return false;

                bool validInput = double.TryParse(enteredNumber, out double enteredNumberDouble);

                if (!validInput || enteredNumberDouble < ConstantsHelper.BodyWeightMinValue || enteredNumberDouble > ConstantsHelper.BodyWeightMaxValue)
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid Input Value", "OK");
                }
                else
                {
                    // Exit loop if user entered valid input
                    userInputDouble = enteredNumberDouble;
                }
            }

            string currentDateTimeString = DateTimeHelper.GetCurrentFormattedDateTime();

            UserWeight userWeight = new()
            {
                BodyWeight = userInputDouble,
                DateTime = currentDateTimeString,
            };

            try
            {
                await UserSettingsViewModel.CreateUserWeightAsync(userWeight);

                // Return true if UserWeight was sucessfully updated
                return true;
            }
            // Return false if there was an error when saving to database
            catch { return false; }
        }

        [RelayCommand]
        private async Task UpdateSetAsync()
        {
            if (OperatingSet is null) return;

            if (!await _context.UpdateItemAsync<Set>(OperatingSet))
            {
                await Shell.Current.DisplayAlert("Error", "Error occured when updating Set.", "OK");
            }
            else
            {
                await GoBack();
            }
        }

        private async Task UpdateWorkoutTemplateAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WorkoutTemplate>(OperatingWorkoutTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout Template.", "OK");
                }
            });
        }

        private async Task CreateNewSetAsync(Set set, int numSets)
        {
            if (numSets < 1 || numSets > 10)
            {
                await Shell.Current.DisplayAlert("Error", "Number of sets must be between 1 and 10", "OK");
                return;
            }

            if (set is null) return;

            if (OperatingWorkoutTemplate is null) return;

            List<string> newSetList;

            if (!string.IsNullOrEmpty(OperatingWorkoutTemplate.SetListOrder))
            {
                // Create list of Ids from comma separated SetListOrder string if it exists
                newSetList = OperatingWorkoutTemplate.SetListOrder.Split(",").ToList();
            }
            else
            {
                // Create empty list if SetListOrder string is null or empty
                newSetList = new();
            }
            
            await ExecuteAsync(async () =>
            {
                for (int i = 0; i < numSets; i++) {
                    await _context.AddItemAsync<Set>(set);
                    newSetList.Add(set.Id.ToString());
                }
            });

            OperatingWorkoutTemplate.SetListOrder = string.Join(",", newSetList);

            await UpdateWorkoutTemplateAsync();

            await GoBack();
        }

        [RelayCommand]
        private async Task AddDefaultPropertyValue(string property)
        {
            string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Default Value", $"Enter Default {property} Value", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

            if (!validInput || enteredNumberInt < ConstantsHelper.SetTrackingMinValue || enteredNumberInt > ConstantsHelper.SetTrackingMaxValue)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Input Value", "OK");
                return;
            }

            ChangeSetPropertyValue(property, enteredNumberInt);
        }

        [RelayCommand]
        private void RemoveDefaultPropertyValue(string property)
        {
            ChangeSetPropertyValue(property, 0);
        }

        [RelayCommand]
        private async Task AddPercentCompletedDefaultValue()
        {
            string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Default Value", "Value must be between 0 and 100.", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

            if (!validInput || enteredNumberInt < 0 || enteredNumberInt > 100)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Input Value", "OK");
                return;
            }

            ChangeSetPropertyValue("PercentCompleted", enteredNumberInt);
        }

        private void ChangeSetPropertyValue(string property, int propertyValue)
        {
            switch (property)
            {
                case "Weight":
                    OperatingSet.Weight = propertyValue;
                    break;

                case "Reps":
                    OperatingSet.Reps = propertyValue;
                    break;

                case "Rir":
                    OperatingSet.Rir = propertyValue;
                    break;

                case "Rpe":
                    OperatingSet.Rpe = propertyValue;
                    break;

                case "Time":
                    OperatingSet.TimeInSeconds = propertyValue;
                    break;

                case "Distance":
                    OperatingSet.Distance = propertyValue;
                    break;

                case "CardioResistance":
                    OperatingSet.CardioResistance = propertyValue;
                    break;

                case "PercentCompleted":
                    OperatingSet.PercentCompleted = propertyValue;
                    break;

                default:
                    return;
            }
        }

        [RelayCommand]
        private async Task DeleteSetAsync()
        {
            if (OperatingSet is null) return;

            bool userClickedDelete = await Shell.Current.DisplayAlert("Delete Set", "Are you sure you want to delete Set?\nThis can not be undone.", "Delete", "Cancel");

            if (!userClickedDelete) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<Set>(OperatingSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to delete Set.", "OK");
                }
            });

            await GoBack();
        }
    }
}
