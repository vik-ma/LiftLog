namespace LocalLiftLog.ViewModels
{
    public partial class UserPreferencesViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferences userSettings;

        [ObservableProperty]
        private UserWeight activeUserWeight;

        public UserPreferencesViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<DefaultEquipmentWeight> defaultEquipmentWeightList = new();

        [ObservableProperty]
        private DefaultEquipmentWeight operatingDefaultEquipmentWeight = new();

        [ObservableProperty]
        private bool displayDefaultEquipmentWeightList;

        [ObservableProperty]
        private WorkoutRoutine activeWorkoutRoutine = new();

        private bool IsInitialized = false;

        private DefaultEquipmentWeightPopupPage Popup;

        [ObservableProperty]
        private List<string> validWeightUnitList = new(ConstantsHelper.ValidWeightUnits.ToList());

        [ObservableProperty]
        private List<string> validDistanceUnitList = new(ConstantsHelper.ValidDistanceUnits.ToList());

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
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task InitializeUserPreferencesAsync()
        {
            if (!IsInitialized)
            {
                await LoadUserPreferencesAsync();
                await LoadActiveUserWeightAsync();
                await LoadDefaultEquipmentWeightsAsync();
                await LoadActiveWorkoutRoutineAsync();
                IsInitialized = true;
            }
        }

        private async Task LoadUserPreferencesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var userPreferences = await _context.GetAllAsync<UserPreferences>();

                if (userPreferences is not null && userPreferences.Any())
                {
                    UserSettings = userPreferences.FirstOrDefault();
                }
                else
                {
                    await CreateUserPreferencesAsync();
                    // TOOD: ADD FUNCTION TO ASK FOR METRIC OR IMPERIAL UNITS
                    await PopulateDefaultEquipmentWeightsAsync();
                }
            });
        }

        public async Task LoadActiveUserWeightAsync()
        {
            if (UserSettings is null) return;

            if (UserSettings.ActiveUserWeightId == 0) return;

            await ExecuteAsync(async () =>
            {
                UserWeight userWeight = await _context.GetItemByKeyAsync<UserWeight>(UserSettings.ActiveUserWeightId);
                
                if (userWeight is null)
                {
                    // Reset ActiveUserWeightId if Id does not exist
                    UserSettings.ActiveUserWeightId = 0;
                    await UpdateUserPreferencesAsync();
                }
                else { ActiveUserWeight = userWeight; }
            });
        }

        public async Task LoadDefaultEquipmentWeightsAsync()
        {
            await ExecuteAsync(async () =>
            {
                DefaultEquipmentWeightList.Clear();

                var defaultWeights = await _context.GetAllAsync<DefaultEquipmentWeight>();

                if (defaultWeights is not null && defaultWeights.Any())
                {
                    defaultWeights ??= new ObservableCollection<DefaultEquipmentWeight>();

                    foreach (var weight in defaultWeights)
                    {
                        DefaultEquipmentWeightList.Add(weight);
                    }
                }
            });
        }

        public async Task LoadActiveWorkoutRoutineAsync()
        {
            if (UserSettings is null) return;

            if (UserSettings.ActiveWorkoutRoutineId == 0) return;

            await ExecuteAsync(async () =>
            {
                WorkoutRoutine workoutRoutine = await _context.GetItemByKeyAsync<WorkoutRoutine>(UserSettings.ActiveWorkoutRoutineId);

                if (workoutRoutine is null)
                {
                    // Reset ActiveWorkoutRoutineId if Id does not exist
                    UserSettings.ActiveWorkoutRoutineId = 0;
                    await UpdateUserPreferencesAsync();
                }
                else { ActiveWorkoutRoutine = workoutRoutine; }
            });
        }

        private async Task CreateUserPreferencesAsync()
        {
            await ExecuteAsync(async () =>
            {
                UserPreferences userPreferences = new();
                await _context.AddItemAsync<UserPreferences>(userPreferences);
                UserSettings = userPreferences;
            });
        }

        [RelayCommand]
        private async Task PopulateDefaultEquipmentWeightsAsync()
        {
            // Default Equipment Weight Values
            DefaultEquipmentWeight defaultBarbellWeight = new()
            {
                Name = "Barbell",
                Weight = 20,
                WeightUnit = "kg"
            };
            DefaultEquipmentWeight defaultDumbbellWeight = new()
            {
                Name = "Dumbbell",
                Weight = 2,
                WeightUnit = "kg"
            };

            await CreateDefaultEquipmentWeightAsync(defaultBarbellWeight);
            await CreateDefaultEquipmentWeightAsync(defaultDumbbellWeight);
        }

        private async Task CreateDefaultEquipmentWeightAsync(DefaultEquipmentWeight defaultEquipmentWeight)
        {
            if (defaultEquipmentWeight is null) return;

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<DefaultEquipmentWeight>(defaultEquipmentWeight);
                DefaultEquipmentWeightList.Add(defaultEquipmentWeight);
            });
        }

        [RelayCommand]
        private async Task AddNewDefaultEquipmentWeightAsync()
        {
            if (OperatingDefaultEquipmentWeight is null) return;

            var (isValid, errorMessage) = OperatingDefaultEquipmentWeight.Validate();

            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            OperatingDefaultEquipmentWeight.WeightUnit = "kg";

            if (OperatingDefaultEquipmentWeight.Id == 0)
            {
                // Create New Default Equipment Weight
                await CreateDefaultEquipmentWeightAsync(OperatingDefaultEquipmentWeight);
            }
            else
            {
                // Update Existing Default Equipment Weight
                await ExecuteAsync(async () =>
                {
                    if (!await _context.UpdateItemAsync<DefaultEquipmentWeight>(OperatingDefaultEquipmentWeight))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when updating Default Equipment Weight.", "OK");
                    }
                });
            }

            OperatingDefaultEquipmentWeight = new();

            ClosePopup();
        }

        [RelayCommand]
        private async Task DeleteDefaultEquipmentWeightAsync(DefaultEquipmentWeight defaultEquipmentWeight)
        {
            if (defaultEquipmentWeight is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<DefaultEquipmentWeight>(defaultEquipmentWeight))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Default Equipment Weight.", "OK");
                }
            });

            await LoadDefaultEquipmentWeightsAsync();
        }

        [RelayCommand]
        public async Task UpdateUserPreferencesAsync()
        {
            if (UserSettings is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<UserPreferences>(UserSettings))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating User Preferences.", "OK");
                }
            });

            OnPropertyChanged(nameof(UserSettings));
        }

        public async Task CreateUserWeightAsync(UserWeight userWeight)
        {
            if (userWeight is null) return;

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<UserWeight>(userWeight);
                UserSettings.ActiveUserWeightId = userWeight.Id;
                await UpdateUserPreferencesAsync();
            });
        }

        [RelayCommand]
        private async Task ToggleIsUsingMetricUnits()
        {
            if (UserSettings is null) return;

            UserSettings.IsUsingMetricUnits = !UserSettings.IsUsingMetricUnits;

            await UpdateUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task ToggleIsUsing24HourClock()
        {
            if (UserSettings is null) return;

            UserSettings.IsUsing24HourClock = !UserSettings.IsUsing24HourClock;

            await UpdateUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task ToggleShowCompletedSetTimestamp()
        {
            if (UserSettings is null) return;

            UserSettings.ShowCompletedSetTimestamp = !UserSettings.ShowCompletedSetTimestamp;

            await UpdateUserPreferencesAsync();
        }

        //[RelayCommand]
        //private async Task DeleteUserPreferencesAsync()
        //{
        //    if (UserSettings is null) return;

        //    await ExecuteAsync(async () =>
        //    {
        //        if (!await _context.DeleteItemAsync<UserPreferences>(UserSettings))
        //        {
        //            await Shell.Current.DisplayAlert("Error", "Error occured when updating User Preferences.", "OK");
        //        }
        //    });

        //    UserSettings = null;

        //    OnPropertyChanged(nameof(UserSettings));
        //}

        [RelayCommand]
        private async Task ResetUserPreferencesAsync()
        {
            if (UserSettings is null) return;

            UserSettings.ResetUserPreferences();

            await UpdateUserPreferencesAsync();
        }

        [RelayCommand]
        private void ToggleDefaultEquipmentWeightList()
        {
            DisplayDefaultEquipmentWeightList = !DisplayDefaultEquipmentWeightList;
        }

        public async Task ResetActiveWorkoutRoutine()
        {
            UserSettings.ActiveWorkoutRoutineId = 0;
            await UpdateUserPreferencesAsync();
            await LoadActiveWorkoutRoutineAsync();
        }

        public async Task ResetActiveUserWeight()
        {
            UserSettings.ActiveUserWeightId = 0;
            await UpdateUserPreferencesAsync();
            await LoadActiveUserWeightAsync();
        }

        #nullable enable
        [RelayCommand]
        private async Task ShowDefaultEquipmentWeightPopup(DefaultEquipmentWeight? defaultEquipmentWeight)
        {
            OperatingDefaultEquipmentWeight = defaultEquipmentWeight ?? new();

            Popup = new DefaultEquipmentWeightPopupPage(this);
            await Shell.Current.ShowPopupAsync(Popup);
        }

        [RelayCommand]
        public void ClosePopup()
        {
            if (Popup is null) return;

            Popup.Close();
        }
    }
}
