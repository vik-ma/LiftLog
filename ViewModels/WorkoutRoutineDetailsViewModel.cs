namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutRoutine), nameof(WorkoutRoutine))]
    public partial class WorkoutRoutineDetailsViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutRoutine workoutRoutine;

        public WorkoutRoutineDetailsViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private ObservableCollection<string> workoutScheduleList = new();

        [ObservableProperty]
        private ObservableCollection<string> dayNameList = new();

        [ObservableProperty]
        private bool isShowingDatePicker;

        [ObservableProperty]
        private DateTime selectedDate;

        private WorkoutRoutineListPopupPage Popup;

        [ObservableProperty]
        private ObservableCollection<WorkoutRoutine> workoutRoutineList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutRoutine> filteredWorkoutRoutineList = new();

        public async Task LoadScheduleAsync()
        {
            LoadNumDaysInSchedule();
            await LoadWorkoutScheduleList();
        }

        private void LoadNumDaysInSchedule()
        {
            if (WorkoutRoutine is null) return;

            DayNameList.Clear();

            if (WorkoutRoutine.IsScheduleWeekly) 
            {
                DayNameList = new(WeekdayHelper.WeekdayList);
            } 
            else
            {
                for (int i = 0; i < WorkoutRoutine.NumDaysInSchedule; i++) 
                {
                    DayNameList.Add($"Day {i+1}");
                }
            }
        }

        private async Task LoadWorkoutScheduleList()
        {
            if (WorkoutRoutine is null) return;

            WorkoutScheduleList.Clear();

            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == WorkoutRoutine.Id;

            IEnumerable<WorkoutTemplateCollection> filteredWtcList = null;
            List<WorkoutTemplatePackage> workoutTemplatePackages = new();
            try
            {
                filteredWtcList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);

                foreach (var wtc in filteredWtcList)
                {
                    WorkoutTemplate workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(wtc.WorkoutTemplateId);

                    if (workoutTemplate is not null) 
                    {
                        WorkoutTemplatePackage workoutTemplatePackage = new()
                        {
                            WorkoutTemplate = workoutTemplate,
                            WorkoutTemplateCollection = wtc
                        };
                        workoutTemplatePackages.Add(workoutTemplatePackage);
                    }
                    else
                    {
                        // Delete WTC if WorkoutTemplate with that Id does not exist
                        await _context.DeleteItemAsync<WorkoutTemplateCollection>(wtc);
                            
                        await Shell.Current.DisplayAlert("Workout Removed", "This Routine's Schedule contained a reference to a Workout Template that no longer exists.\nThis workout has been removed from the Schedule.", "OK");
                    }
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Schedule.", "OK");
                return;
            }

            Dictionary<int, List<WorkoutTemplatePackage>> wtcByDayDictionary = workoutTemplatePackages
               .GroupBy(item => item.WorkoutTemplateCollection.Day)
               .ToDictionary(group => group.Key, group => group.ToList());

            for (int i = 0; i < WorkoutRoutine.NumDaysInSchedule; i++)
            {
                string dayString;

                if (wtcByDayDictionary.TryGetValue(i, out List<WorkoutTemplatePackage> value))
                {
                    dayString = string.Join(", ", value.Select(item => item.WorkoutTemplate.Name));
                }
                else
                {
                    dayString = "No Workout";
                }

                WorkoutScheduleList.Add(dayString);
            }
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

        private async Task DeleteWorkoutTemplateCollectionsByWorkoutRoutineId()
        {
            if (WorkoutRoutine is null) return;

            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == WorkoutRoutine.Id;

            IEnumerable<WorkoutTemplateCollection> filteredWtcList = null;
            try
            {
                filteredWtcList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Workout Template Collections.", "OK");
            }

            foreach (var item in filteredWtcList)
            {
                await ExecuteAsync(async () =>
                {
                    if (!await _context.DeleteItemAsync<WorkoutTemplateCollection>(item))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                    }
                });
            }
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task UpdateWorkoutRoutineAsync()
        {
            var (isValid, errorMessage) = WorkoutRoutine.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WorkoutRoutine>(WorkoutRoutine))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating WorkoutRoutine.", "OK");
                    return;
                }
            });

            OnPropertyChanged(nameof(WorkoutRoutine));
            IsEditing = false;
        }

        [RelayCommand]
        private void EnableEditing()
        {
            IsEditing = true;
        }

        [RelayCommand]
        private void DisableEditing()
        {
            IsEditing = false;
        }

        [RelayCommand]
        private void ShowDatePicker()
        {
            IsShowingDatePicker = true;
        }

        [RelayCommand]
        private void HideDatePicker()
        {
            IsShowingDatePicker = false;
        }

        [RelayCommand]
        private async Task SaveStartDay()
        {
            if (WorkoutRoutine is null) return;

            WorkoutRoutine.CustomScheduleStartDate = DateTimeHelper.FormatDateTimeToYmdString(SelectedDate);

            await UpdateWorkoutRoutineAsync();

            IsShowingDatePicker = false;
        }

        [RelayCommand]
        private async Task GoToSchedulePage()
        {
            if (WorkoutRoutine is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutRoutine"] = WorkoutRoutine
            };

            await Shell.Current.GoToAsync($"{nameof(SchedulePage)}?Id={WorkoutRoutine.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task CreateWeeklySchedule()
        {
            if (WorkoutRoutine is null || WorkoutRoutine.IsScheduleWeekly) return;

            WorkoutRoutine.IsScheduleWeekly = true;
            WorkoutRoutine.NumDaysInSchedule = 7;

            await UpdateWorkoutRoutineAsync();

            await LoadScheduleAsync();
        }

        [RelayCommand]
        private async Task CreateCustomSchedule()
        {
            if (WorkoutRoutine is null) return;

            var (userEnteredValidNumber, numberOfDays) = await ShowNumDaysPrompt();

            if (!userEnteredValidNumber) return;

            WorkoutRoutine.IsScheduleWeekly = false;
            WorkoutRoutine.NumDaysInSchedule = numberOfDays;

            await UpdateWorkoutRoutineAsync();

            await LoadScheduleAsync();
        }

        private static async Task<(bool, int)> ShowNumDaysPrompt()
        {
            string enteredNumber = await Shell.Current.DisplayPromptAsync("Number Of Days In Schedule", "How many days should the schedule contain?\n(Must be between 2 and 14)\n", "OK", "Cancel");

            if (enteredNumber == null) return (false, 0);

            bool validInput = int.TryParse(enteredNumber, out int numberOfDays);

            if (!validInput || numberOfDays < 2 || numberOfDays > 14)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid input.", "OK");
                return (false, 0);
            }

            return (true, numberOfDays);
        }

        public async Task SetCustomScheduleStartDate(DateTime selectedDate)
        {
            if (WorkoutRoutine is null || WorkoutRoutine.IsScheduleWeekly) return;

            string ymdDateString = DateTimeHelper.FormatDateTimeToYmdString(selectedDate);

            WorkoutRoutine.CustomScheduleStartDate = ymdDateString;

            await UpdateWorkoutRoutineAsync();

            OnPropertyChanged(nameof(WorkoutRoutine));
        }

        [RelayCommand]
        private async Task ResetSchedule()
        {
            if (WorkoutRoutine is null) return;

            bool userClickedDelete = await Shell.Current.DisplayAlert("Reset Schedule", "Are you sure you want to completely reset the Workout Routine Schedule?", "Reset", "Cancel");

            if (!userClickedDelete) return;

            await DeleteWorkoutTemplateCollectionsByWorkoutRoutineId();

            WorkoutRoutine.ResetSchedule();

            await UpdateWorkoutRoutineAsync();

            OnPropertyChanged(nameof(WorkoutRoutine));

            await LoadScheduleAsync();
        }
        public async Task LoadWorkoutRoutinesAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutRoutineList.Clear();

                var workoutRoutines = await _context.GetAllAsync<WorkoutRoutine>();

                if (workoutRoutines is not null && workoutRoutines.Any())
                {
                    workoutRoutines ??= new ObservableCollection<WorkoutRoutine>();

                    foreach (var routine in workoutRoutines)
                    {
                        WorkoutRoutineList.Add(routine);
                    }
                }

                FilteredWorkoutRoutineList = new(WorkoutRoutineList);
            });
        }

        [RelayCommand]
        private async Task ShowWorkoutRoutineListPopup()
        {
            if (WorkoutRoutine is null) return;

            await LoadWorkoutRoutinesAsync();

            Popup = new WorkoutRoutineListPopupPage(this);
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
