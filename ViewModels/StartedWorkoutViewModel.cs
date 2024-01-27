namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(CompletedWorkout), nameof(CompletedWorkout))]
    public partial class StartedWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        [ObservableProperty]
        private CompletedWorkout completedWorkout;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        private readonly List<int> SetListIdOrder = new();

        private readonly HashSet<string> SetPropertyHashSet;

        public StartedWorkoutViewModel(DatabaseContext context, UserPreferencesViewModel userSettings)
        {
            _context = context;
            userSettingsViewModel = userSettings;
            SetPropertyHashSet = SetProperties.SetPropertyHashSet;
        }

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        private ObservableCollection<SetPackage> setList = new();

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

        private void LoadSetListIdOrder()
        {
            if (WorkoutTemplate is null) return;

            if (string.IsNullOrEmpty(WorkoutTemplate.SetListOrder)) return;

            string[] setList = WorkoutTemplate.SetListOrder.Split(',');

            foreach (string s in setList)
            {
                if (int.TryParse(s, out int setId))
                {
                    SetListIdOrder.Add(setId);
                }
            }

            OnPropertyChanged(nameof(WorkoutTemplate));
        }

        public async Task LoadSetListFromWorkoutTemplateIdAsync()
        {
            if (CompletedWorkout is null) return;

            // Load WorkoutTemplate from WorkoutTemplateId
            WorkoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(CompletedWorkout.WorkoutTemplateId);

            if (WorkoutTemplate is null) return;

            LoadSetListIdOrder();

            SetList.Clear();

            List<SetPackage> setPackageList = new();

            Expression<Func<SetTemplate, bool>> predicateSetTemplate = entity => entity.WorkoutTemplateId == WorkoutTemplate.Id;
            Expression<Func<CompletedSet, bool>> predicateCompletedSet = entity => entity.CompletedWorkoutId == CompletedWorkout.Id;

            try
            {
                var filteredSetTemplateList = await _context.GetFilteredAsync<SetTemplate>(predicateSetTemplate);

                IEnumerable<CompletedSet> filteredCompletedSetList = null;
                if (CompletedWorkout.Id != 0)
                {
                    // Get any CompletedSets if CompletedWorkout has been saved
                    filteredCompletedSetList = await _context.GetFilteredAsync<CompletedSet>(predicateCompletedSet);
                }

                foreach (var item in filteredSetTemplateList)
                {
                    SetPackage setPackage = new() 
                    { 
                        SetTemplate = item,
                        // Set the saved CompletedSet to the corresponding SetTemplate if it exists
                        // Otherwise create a new CompletedSet object
                        CompletedSet = filteredCompletedSetList?.FirstOrDefault(c => c.SetTemplateId == item.Id) ?? new CompletedSet
                        {
                            CompletedWorkoutId = CompletedWorkout.Id
                        }
                    };

                    // Load SetTemplate Default Values if Set is not completed
                    if (!setPackage.CompletedSet.IsCompleted) setPackage = LoadSetTemplateDefaultValues(setPackage);

                    setPackageList.Add(setPackage);
                }

                if (setPackageList.Any())
                {
                    // Sort the SetList by its SetListIdOrder
                    SetList = new ObservableCollection<SetPackage>(setPackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.SetTemplate.Id)));
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                return;
            }
        }

        private static SetPackage LoadSetTemplateDefaultValues(SetPackage setPackage)
        {
            // Assign SetTemplate Default Values to CompletedSet if any are set
            if (setPackage.SetTemplate.DefaultWeightValue != 0) setPackage.CompletedSet.Weight = setPackage.SetTemplate.DefaultWeightValue;
            if (setPackage.SetTemplate.DefaultRepsValue != 0) setPackage.CompletedSet.Reps = setPackage.SetTemplate.DefaultRepsValue;
            if (setPackage.SetTemplate.DefaultRirValue != 0) setPackage.CompletedSet.Rir = setPackage.SetTemplate.DefaultRirValue;
            if (setPackage.SetTemplate.DefaultRpeValue != 0) setPackage.CompletedSet.Rpe = setPackage.SetTemplate.DefaultRpeValue;
            if (setPackage.SetTemplate.DefaultTimeValue != 0) setPackage.CompletedSet.Time = setPackage.SetTemplate.DefaultTimeValue;
            if (setPackage.SetTemplate.DefaultDistanceValue != 0) setPackage.CompletedSet.Distance = setPackage.SetTemplate.DefaultDistanceValue;
            if (setPackage.SetTemplate.DefaultCardioResistanceValue != 0) setPackage.CompletedSet.CardioResistance = setPackage.SetTemplate.DefaultCardioResistanceValue;
            if (setPackage.SetTemplate.DefaultPercentCompletedValue != 0) setPackage.CompletedSet.PercentCompleted = setPackage.SetTemplate.DefaultPercentCompletedValue;

            return setPackage;
        }

        [RelayCommand]
        private async Task GoToWorkoutTemplateDetails()
        {
            if (WorkoutTemplate is null)
            {
                await Shell.Current.DisplayAlert("Error", "Workout does not exist", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutTemplate"] = WorkoutTemplate
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?Id={WorkoutTemplate.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task GoToCreateSetTemplatePage(SetTemplate selectedSetTemplate)
        {
            if (selectedSetTemplate is null) return;

            SetWorkoutTemplatePackage package = new()
            {
                WorkoutTemplate = WorkoutTemplate,
                SetTemplate = selectedSetTemplate,
                IsEditing = true
            };

            var navigationParameter = new Dictionary<string, object>
            {
                ["SetWorkoutTemplatePackage"] = package
            };

            await Shell.Current.GoToAsync($"{nameof(CreateSetTemplatePage)}?Id={WorkoutTemplate.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task SaveDate()
        {
            if (CompletedWorkout is null) return;

            string ymdDate = DateTimeHelper.FormatDateTimeToYmdString(SelectedDate);

            CompletedWorkout.Date = ymdDate;

            await UpdateCompletedWorkoutAsync();

            OnPropertyChanged(nameof(CompletedWorkout));
        }

        private async Task UpdateCompletedWorkoutAsync()
        {
            if (CompletedWorkout is null) return;

            if (await _context.ItemExistsByKeyAsync<CompletedWorkout>(CompletedWorkout.Id))
            {
                // Update CompletedWorkout if it is already created
                await ExecuteAsync(async () =>
                {
                    if (!await _context.UpdateItemAsync<CompletedWorkout>(CompletedWorkout))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout.", "OK");
                    }
                });
            }
            // Create new CompletedWorkout if not created
            else await CreateCompletedWorkoutAsync(); 
        }

        private async Task CreateCompletedWorkoutAsync()
        {
            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<CompletedWorkout>(CompletedWorkout);
            });
        }

        [RelayCommand]
        private static void ShowEditSetProperties(SetPackage setPackage)
        {
            if (setPackage is null) return;

            setPackage.IsEditingSetProperties = true;
        }

        [RelayCommand]
        private static void HideEditSetProperties(SetPackage setPackage)
        {
            if (setPackage is null) return;

            setPackage.IsEditingSetProperties = false;
        }

        private bool ValidateUserWeightExists()
        {
            if (UserSettingsViewModel.ActiveUserWeight is null) return false;

            if (UserSettingsViewModel.ActiveUserWeight.BodyWeight == 0) return false;

            return true;
        }
        
        private async Task<bool> ShowUpdateWeightPrompt(SetPackage setPackage)
        {
            bool userClickedSetBodyWeight = await Shell.Current.DisplayAlert("No Body Weight Set", "This Set is configured to add Body Weight to total Weight, but no Body Weight has been set!\n\nYou must either set your Body Weight or remove the property from the Set.", "Set Body Weight", "Remove Body Weight Property");
        
            if (userClickedSetBodyWeight)
            {
                int userInputInt = 0;

                // Keep asking for input until valid input has been entered or user clicks Cancel
                while (userInputInt == 0)
                {
                    string enteredNumber = await Shell.Current.DisplayPromptAsync("Enter Body Weight", "Enter your current body weight:\n", "OK", "Cancel");

                    // Return false if user clicks Cancel
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

                await UserSettingsViewModel.CreateUserWeightAsync(userWeight);

                // Return true if Body Weight was updated
                return true;
            }

            // Update IsUsingBodyWeightAsWeight property in SetTemplate to false
            setPackage.SetTemplate.IsUsingBodyWeightAsWeight = false;
            await UpdateSetTemplateAsync(setPackage.SetTemplate);

            return true;
        }

        private async Task UpdateSetTemplateAsync(SetTemplate setTemplate)
        {
            if (setTemplate is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<SetTemplate>(setTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to update Set.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task SaveSetAsync(SetPackage setPackage)
        {
            if (setPackage is null) return;

            if (setPackage.SetTemplate.IsUsingBodyWeightAsWeight && !ValidateUserWeightExists())
            {
                // Exit function if user clicked cancel in ShowUpdateWeightPrompt function
                if (!await ShowUpdateWeightPrompt(setPackage)) return;
            }

            setPackage.CompletedSet.IsCompleted = true;
            setPackage.CompletedSet.TimeCompleted = DateTimeHelper.GetCurrentFormattedDateTime();
            setPackage.CompletedSet.SetTemplateId = setPackage.SetTemplate.Id;
            
            await ExecuteAsync(async () =>
            {
                if (!await _context.AddItemAsync<CompletedSet>(setPackage.CompletedSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to save Set.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task UpdateSetAsync(CompletedSet completedSet)
        {
            if (completedSet is null) return;

            if (!completedSet.IsCompleted) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CompletedSet>(completedSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to update Set.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task ToggleShowCompletedSetTimeStamp()
        {
            UserSettingsViewModel.UserSettings.ShowCompletedSetTimestamp = !UserSettingsViewModel.UserSettings.ShowCompletedSetTimestamp;

            await UserSettingsViewModel.UpdateUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task Toggle24HourClock()
        {
            UserSettingsViewModel.UserSettings.IsUsing24HourClock = !UserSettingsViewModel.UserSettings.IsUsing24HourClock;

            await UserSettingsViewModel.UpdateUserPreferencesAsync();
        }
    }
}
