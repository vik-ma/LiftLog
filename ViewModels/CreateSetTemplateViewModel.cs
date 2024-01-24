﻿namespace LocalLiftLog.ViewModels
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
        private SetTemplate operatingSetTemplate;

        [ObservableProperty]
        private WorkoutTemplate operatingWorkoutTemplate;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string defaultWeightUnit;

        [ObservableProperty]
        private string defaultDistanceUnit;

        public CreateSetTemplateViewModel(DatabaseContext context, ExerciseDataManager exerciseData, UserPreferencesViewModel userSettings)
        {
            _context = context;
            _exerciseData = exerciseData;
            userSettingsViewModel = userSettings;
            SetDefaultUnitValues();
        }

        [ObservableProperty]
        public List<Exercise> exerciseList = new();

        [ObservableProperty]
        public ObservableCollection<Exercise> filteredExerciseList = new();

        [ObservableProperty]
        private int newSetTemplateNumSets = 1;

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

        public void InitializeSetWorkoutTemplatePackage()
        {
            OperatingSetTemplate = SetWorkoutTemplatePackage.SetTemplate;
            OperatingWorkoutTemplate = SetWorkoutTemplatePackage.WorkoutTemplate;
            IsEditing = SetWorkoutTemplatePackage.IsEditing;

            // TODO: LOAD EXERCISE FROM ID + HANDLE NOT EXIST
            //if (IsEditing) NewSetTemplateSelectedExerciseName = OperatingSetTemplate.ExerciseName;
        }

        private void SetDefaultUnitValues()
        {
            DefaultWeightUnit = UserSettingsViewModel.UserSettings.IsUsingMetricUnits ? ConstantsHelper.DefaultWeightUnitMetricTrue : ConstantsHelper.DefaultWeightUnitMetricFalse;
            DefaultDistanceUnit = UserSettingsViewModel.UserSettings.IsUsingMetricUnits ? ConstantsHelper.DefaultDistanceUnitMetricTrue : ConstantsHelper.DefaultDistanceUnitMetricFalse;
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
        private async Task SaveSetTemplateAsync()
        {
            if (OperatingSetTemplate is null || OperatingWorkoutTemplate is null) return;

            if (IsEditing)
            {
                // If editing existing SetTemplate
                await UpdateSetTemplateAsync();
            }
            else
            {
                // If creating new SetTemplate(s)
                SetTemplate newSetTemplate = new()
                {
                    WorkoutTemplateId = OperatingWorkoutTemplate.Id,
                    ExerciseId = SelectedExercise.Id,
                    Note = OperatingSetTemplate.Note,
                    IsTrackingWeight = OperatingSetTemplate.IsTrackingWeight,
                    IsTrackingReps = OperatingSetTemplate.IsTrackingReps,
                    IsTrackingRir = OperatingSetTemplate.IsTrackingRir,
                    IsTrackingRpe = OperatingSetTemplate.IsTrackingRpe,
                    IsTrackingTime = OperatingSetTemplate.IsTrackingTime,
                    IsTrackingDistance = OperatingSetTemplate.IsTrackingDistance,
                    IsTrackingCardioResistance = OperatingSetTemplate.IsTrackingCardioResistance,
                    IsUsingBodyWeightAsWeight = OperatingSetTemplate.IsUsingBodyWeightAsWeight,
                };

                int numSets = NewSetTemplateNumSets;

                // Validate SetTemplate properties
                var (isSetTemplateValid, errorMessage) = newSetTemplate.ValidateSetTemplate();
                if (!isSetTemplateValid)
                {
                    await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                    return;
                }

                if (newSetTemplate.IsUsingBodyWeightAsWeight)
                {
                    // If no Active UserWeight is set, but IsUsingBodyWeightAsWeight has been checked
                    if (UserSettingsViewModel.UserSettings.ActiveUserWeightId == 0)
                    {
                        bool userUpdatedUserWeight = await ShowUpdateWeightPrompt();

                        // Exit function if user did not enter a valid weight
                        if (!userUpdatedUserWeight) return;
                    }

                    newSetTemplate = DisableBodyWeightTrackingIfNotTrackingWeight(newSetTemplate);
                }
                
                await CreateNewSetTemplateAsync(newSetTemplate, numSets);
            }
        }

        private static SetTemplate DisableBodyWeightTrackingIfNotTrackingWeight(SetTemplate setTemplate) 
        {
            if (!setTemplate.IsTrackingWeight)
            {
                // Set IsUsingBodyWeightAsWeight to false if it is checked even if Weight is not tracked
                setTemplate.IsUsingBodyWeightAsWeight = false;
            }
            return setTemplate;
        }

        private async Task<bool> ShowUpdateWeightPrompt()
        {
            bool userClickedSetBodyWeight = await Shell.Current.DisplayAlert("No Body Weight Set", "A body weight needs to be set in order to add body weight to total weight.\n\nDo you want to set a body weight?", "Set Body Weight", "Cancel");

            if (!userClickedSetBodyWeight) return false;

            int userInputInt = 0;

            // Keep asking for input until valid input has been entered or user clicks Cancel
            while (userInputInt == 0)
            {
                string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Body Weight", "Enter your current body weight:\n", "OK", "Cancel");

                // Return false if user clicked Cancel
                if (enteredNumber == null) return false;

                bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

                if (!validInput || enteredNumberInt < ConstantsHelper.BodyWeightInputMinValue || enteredNumberInt > ConstantsHelper.BodyWeightMaxValue)
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid Input Value.\n", "OK");
                }
                else
                {
                    // Exit loop if user entered valid input
                    userInputInt = enteredNumberInt;
                }
            }

            string currentDateTimeString = DateTimeHelper.GetCurrentFormattedDateTime();

            UserWeight userWeight = new()
            {
                BodyWeight = userInputInt,
                DateTime = currentDateTimeString
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
        private async Task UpdateSetTemplateAsync()
        {
            if (OperatingSetTemplate is null) return;

            // Validate SetTemplate properties
            var (isSetTemplateValid, errorMessage) = OperatingSetTemplate.ValidateSetTemplate();
            if (!isSetTemplateValid)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            if (OperatingSetTemplate.IsUsingBodyWeightAsWeight)
            {
                // If no Active UserWeight is set, but IsUsingBodyWeightAsWeight has been checked
                if (UserSettingsViewModel.UserSettings.ActiveUserWeightId == 0)
                {
                    bool userUpdatedUserWeight = await ShowUpdateWeightPrompt();

                    // Exit function if user did not enter a valid weight
                    if (!userUpdatedUserWeight) return;
                }

                OperatingSetTemplate = DisableBodyWeightTrackingIfNotTrackingWeight(OperatingSetTemplate);
            }

            if (!await _context.UpdateItemAsync<SetTemplate>(OperatingSetTemplate))
            {
                await Shell.Current.DisplayAlert("Error", "Error occured when updating Set Template.", "OK");
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

        private async Task CreateNewSetTemplateAsync(SetTemplate setTemplate, int numSets)
        {
            if (numSets < 1 || numSets > 10)
            {
                await Shell.Current.DisplayAlert("Error", "Number of sets must be between 1 and 10!", "OK");
                return;
            }

            if (setTemplate == null) return;

            if (OperatingWorkoutTemplate is null) return;

            List<string> newSetList;

            if (!string.IsNullOrEmpty(OperatingWorkoutTemplate.SetListOrder))
            {
                // Create list of Ids from comma separated SetListOrder string if it exists
                newSetList = OperatingWorkoutTemplate.SetListOrder.Split(',').ToList();
            }
            else
            {
                // Create empty list if SetListOrder string is null or empty
                newSetList = new();
            }
            
            await ExecuteAsync(async () =>
            {
                for (int i = 0; i < numSets; i++) {
                    await _context.AddItemAsync<SetTemplate>(setTemplate);
                    newSetList.Add(setTemplate.Id.ToString());
                }
            });

            OperatingWorkoutTemplate.SetListOrder = string.Join(",", newSetList);

            await UpdateWorkoutTemplateAsync();

            await GoBack();
        }

        [RelayCommand]
        private async Task AddDefaultPropertyValue(string property)
        {
            string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Default Value", $"Value must be between {ConstantsHelper.SetTemplateDefaultInputMinValue} and {ConstantsHelper.SetTemplateDefaultMaxValue}.\n", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

            if (!validInput || enteredNumberInt < ConstantsHelper.SetTemplateDefaultInputMinValue || enteredNumberInt > ConstantsHelper.SetTemplateDefaultMaxValue)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Input Value.\n", "OK");
                return;
            }

            ChangeSetTemplatePropertyValue(property, enteredNumberInt);
        }

        [RelayCommand]
        private void RemoveDefaultPropertyValue(string property)
        {
            ChangeSetTemplatePropertyValue(property, 0);
        }

        [RelayCommand]
        private async Task AddPercentCompletedDefaultValue()
        {
            string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Default Value", $"Value must be between {ConstantsHelper.PercentInputMinValue} and {ConstantsHelper.PercentInputMaxValue}.\n", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

            if (!validInput || enteredNumberInt < ConstantsHelper.PercentInputMinValue || enteredNumberInt > ConstantsHelper.PercentInputMaxValue)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Input Value.\n", "OK");
                return;
            }

            ChangeSetTemplatePropertyValue("PercentCompleted", enteredNumberInt);
        }

        private void ChangeSetTemplatePropertyValue(string property, int propertyValue)
        {
            switch (property)
            {
                case "Weight":
                    OperatingSetTemplate.DefaultWeightValue = propertyValue;
                    break;

                case "Reps":
                    OperatingSetTemplate.DefaultRepsValue = propertyValue;
                    break;

                case "Rir":
                    OperatingSetTemplate.DefaultRirValue = propertyValue;
                    break;

                case "Rpe":
                    OperatingSetTemplate.DefaultRpeValue = propertyValue;
                    break;

                case "Time":
                    OperatingSetTemplate.DefaultTimeValue = propertyValue;
                    break;

                case "Distance":
                    OperatingSetTemplate.DefaultDistanceValue = propertyValue;
                    break;

                case "CardioResistance":
                    OperatingSetTemplate.DefaultCardioResistanceValue = propertyValue;
                    break;

                case "PercentCompleted":
                    OperatingSetTemplate.DefaultPercentCompletedValue = propertyValue;
                    break;

                default:
                    return;
            }
        }
    }
}
