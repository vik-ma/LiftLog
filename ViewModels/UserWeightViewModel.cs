namespace LocalLiftLog.ViewModels
{
    public partial class UserWeightViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        public UserWeightViewModel(DatabaseContext context, UserPreferencesViewModel userSettings)
        {
            _context = context;
            userSettingsViewModel = userSettings;
        }

        [ObservableProperty]
        public ObservableCollection<UserWeight> userWeightList = new();

        [ObservableProperty]
        public string newWeightInput;

        [ObservableProperty]
        public UserWeight latestWeight;

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

        public async Task LoadUserWeightListAsync()
        {
            await ExecuteAsync(async () =>
            {
                UserWeightList.Clear();

                var weightList = await _context.GetAllAsync<UserWeight>();

                if (weightList is not null && weightList.Any())
                {
                    weightList ??= new ObservableCollection<UserWeight>();

                    foreach (var set in weightList)
                    {
                        UserWeightList.Add(set);
                    }

                    LatestWeight = weightList.OrderByDescending(n => n.Id).FirstOrDefault();
                    
                    await UpdateActiveUserWeightId(LatestWeight.Id);
                }
                else 
                {
                    LatestWeight = null;

                    if (UserSettingsViewModel.UserSettings.ActiveUserWeightId != 0)
                    {
                        // Reset ActiveUserWeightId if one was set
                        await UpdateActiveUserWeightId(0);
                    }
                }
            });
        }

        private async Task UpdateActiveUserWeightId(int id)
        {
            UserSettingsViewModel.UserSettings.ActiveUserWeightId = id;
            UserSettingsViewModel.ActiveUserWeight = LatestWeight;

            await UserSettingsViewModel.UpdateUserPreferencesAsync();
            await UserSettingsViewModel.LoadActiveUserWeightAsync();
        }

        private async Task CreateUserWeightAsync(UserWeight userWeight)
        {
            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<UserWeight>(userWeight);
                UserWeightList.Add(userWeight);
            });
        }

        [RelayCommand]
        private async Task AddWeight()
        {
            if (string.IsNullOrWhiteSpace(NewWeightInput)) return;

            bool validInput = double.TryParse(NewWeightInput, out double weightInputDouble);

            if (!validInput || weightInputDouble < ConstantsHelper.BodyWeightInputMinValue || weightInputDouble > ConstantsHelper.BodyWeightMaxValue)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Weight Input.\n", "OK");
                return;
            }

            string currentDateTimeString = DateTimeHelper.GetCurrentFormattedDateTime();

            double roundedInput = Math.Round(weightInputDouble, 2, MidpointRounding.AwayFromZero);

            UserWeight userWeight = new()
            {
                BodyWeight = roundedInput,
                DateTime = currentDateTimeString
            };

            await CreateUserWeightAsync(userWeight);

            await LoadUserWeightListAsync();

            NewWeightInput = null;
        }

        [RelayCommand]
        private async Task DeleteWeightAsync(UserWeight userWeight)
        {
            if (userWeight is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<UserWeight>(userWeight))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Weight.", "OK");
                }

                // Reset ActiveUserWeight in UserSettings if deleted UserWeight is the active one
                if (userWeight.Id == UserSettingsViewModel.UserSettings.ActiveUserWeightId) 
                {
                    await UserSettingsViewModel.ResetActiveUserWeight();
                }
            });

            await LoadUserWeightListAsync();
        }
    }
}
