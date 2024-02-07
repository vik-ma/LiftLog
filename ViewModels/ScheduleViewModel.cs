namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutRoutine), nameof(WorkoutRoutine))]
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutRoutine workoutRoutine;

        public ScheduleViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day1WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day2WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day3WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day4WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day5WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day6WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day7WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day8WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day9WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day10WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day11WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day12WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day13WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplatePackage> day14WorkoutTemplatePackageList = new();

        [ObservableProperty]
        private int selectedDay;

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new(); 

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> filteredWorkoutTemplateList = new();

        private WorkoutTemplateListPopupPage Popup;

        [ObservableProperty]
        private string popupTitle;

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

        public async Task LoadWorkoutTemplateCollectionsAsync()
        {
            if (WorkoutRoutine is null) return;

            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == WorkoutRoutine.Id;

            IEnumerable<WorkoutTemplateCollection> filteredWtcList = null;
            try
            {
                filteredWtcList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);
                LoadWorkoutTemplateCollectionsForEachDay(filteredWtcList);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
            }
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

        private async void LoadWorkoutTemplateCollectionsForEachDay(IEnumerable<WorkoutTemplateCollection> filteredWtcList)
        {
            Day1WorkoutTemplatePackageList.Clear();
            Day2WorkoutTemplatePackageList.Clear();
            Day3WorkoutTemplatePackageList.Clear();
            Day4WorkoutTemplatePackageList.Clear();
            Day5WorkoutTemplatePackageList.Clear();
            Day6WorkoutTemplatePackageList.Clear();
            Day7WorkoutTemplatePackageList.Clear();
            Day8WorkoutTemplatePackageList.Clear();
            Day9WorkoutTemplatePackageList.Clear();
            Day10WorkoutTemplatePackageList.Clear();
            Day11WorkoutTemplatePackageList.Clear();
            Day12WorkoutTemplatePackageList.Clear();
            Day13WorkoutTemplatePackageList.Clear();
            Day14WorkoutTemplatePackageList.Clear();

            foreach (var item in filteredWtcList)
            {
                WorkoutTemplate workoutTemplate = null;

                await ExecuteAsync(async () =>
                {
                    workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(item.WorkoutTemplateId);
                });

                if (workoutTemplate is null)
                {
                    // Delete WorkoutTemplateCollection if it references a WorkoutTemplate whose Id does not exist
                    await ExecuteAsync(async () =>
                    {
                        if (!await _context.DeleteItemAsync<WorkoutTemplateCollection>(item))
                        {
                            await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                        }
                    });
                    return;
                }

                WorkoutTemplatePackage workoutTemplatePackage = new()
                {
                    WorkoutTemplate = workoutTemplate,
                    WorkoutTemplateCollection = item
                };

                int day = item.Day;

                switch (day)
                {
                    case 0:
                        Day1WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 1:
                        Day2WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 2:
                        Day3WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 3:
                        Day4WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 4:
                        Day5WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 5:
                        Day6WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 6:
                        Day7WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 7:
                        Day8WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 8:
                        Day9WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 9:
                        Day10WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 10:
                        Day11WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 11:
                        Day12WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 12:
                        Day13WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    case 13:
                        Day14WorkoutTemplatePackageList.Add(workoutTemplatePackage);
                        break;

                    default:
                        await Shell.Current.DisplayAlert("Error", "Invalid Day.", "OK");
                        break;
                }
            }
        }

        [RelayCommand]
        private async Task DeleteWorkoutTemplateCollectionAsync(WorkoutTemplateCollection workoutTemplateCollection)
        {
            if (workoutTemplateCollection is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<WorkoutTemplateCollection>(workoutTemplateCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                }
            });

            await LoadWorkoutTemplateCollectionsAsync();
        }

        public async Task AddWorkoutTemplateCollectionToDay(WorkoutTemplate workoutTemplate)
        {
            if (workoutTemplate is null) return;

            if (WorkoutRoutine is null) return;

            await ExecuteAsync(async () =>
            {
                WorkoutTemplateCollection workoutCollection = new()
                {
                    Day = SelectedDay,
                    WorkoutTemplateId = workoutTemplate.Id,
                    WorkoutRoutineId = WorkoutRoutine.Id,
                };
                await _context.AddItemAsync<WorkoutTemplateCollection>(workoutCollection);
            });

            await LoadWorkoutTemplateCollectionsAsync();
        }

        [RelayCommand]
        private static async Task GoToWorkoutTemplate()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutTemplateListPage)}");
        }

        [RelayCommand]
        private async Task ShowWorkoutTemplateListPopup(string dayString)
        {
            if (WorkoutRoutine is null) return;

            int day;

            if (int.TryParse(dayString, out int intValue))
            {
                day = intValue;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Day String", "OK");
                return;
            }

            if (day < 0 || day > 13)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Day", "OK");
                return;
            }

            SelectedDay = day;

            string displayedDay = DateTimeHelper.ConvertScheduleDayIntToDayString(WorkoutRoutine.IsScheduleWeekly, day);

            PopupTitle = $"Add Workout To {displayedDay}";

            await LoadWorkoutTemplatesAsync();

            Popup = new WorkoutTemplateListPopupPage("Schedule", null, this, null);
            await Shell.Current.ShowPopupAsync(Popup);
        }

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

        [RelayCommand]
        private async Task UpdateWorkoutRoutineAsync()
        {
            if (WorkoutRoutine is null) return;

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
        }

        [RelayCommand]
        private async Task CreateWeeklySchedule()
        {
            if (WorkoutRoutine is null || WorkoutRoutine.IsScheduleWeekly) return;

            WorkoutRoutine.IsScheduleWeekly = true;
            WorkoutRoutine.NumDaysInSchedule = 7;

            await UpdateWorkoutRoutineAsync();

            await LoadWorkoutTemplateCollectionsAsync();

            OnPropertyChanged(nameof(WorkoutRoutine));
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

            await LoadWorkoutTemplateCollectionsAsync();

            OnPropertyChanged(nameof(WorkoutRoutine));
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

            await DeleteAllWorkoutTemplateCollectionsForWorkoutRoutine();

            WorkoutRoutine.ResetSchedule();

            await UpdateWorkoutRoutineAsync();

            OnPropertyChanged(nameof(WorkoutRoutine));
        }

        private async Task DeleteAllWorkoutTemplateCollectionsForWorkoutRoutine()
        {
            if (WorkoutRoutine is null) return;

            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == WorkoutRoutine.Id;

            IEnumerable<WorkoutTemplateCollection> filteredWtcList = null;

            await ExecuteAsync(async () =>
            {
                filteredWtcList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);

                foreach (var wtc in filteredWtcList)
                {
                    await DeleteWorkoutTemplateCollectionAsync(wtc);
                }
            });

            await LoadWorkoutTemplateCollectionsAsync();
        }
    }
}
