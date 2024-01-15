namespace LocalLiftLog.ViewModels
{
    public partial class UserPreferencesViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferences userSettings;

        [ObservableProperty]
        private UserWeight userWeight;

        public UserPreferencesViewModel(DatabaseContext context)
        {
            _context = context;
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
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task InitializeUserPreferencesAsync()
        {
            await LoadUserPreferencesAsync();
            await LoadActiveUserWeightAsync();
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
                    await CreateDefaultEquipmentWeightsAsync();
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
                else { UserWeight = userWeight; }
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

        private async Task CreateDefaultEquipmentWeightsAsync()
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

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<DefaultEquipmentWeight>(defaultBarbellWeight);
                await _context.AddItemAsync<DefaultEquipmentWeight>(defaultDumbbellWeight);
            });
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
    }
}
