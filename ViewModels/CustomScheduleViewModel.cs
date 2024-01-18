namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(CustomSchedule), nameof(CustomSchedule))]
    public partial class CustomScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private CustomSchedule customSchedule;

        public CustomScheduleViewModel(DatabaseContext context)
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
        private bool showWorkoutTemplateList = false;

        [ObservableProperty]
        private int selectedDay;

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

        [ObservableProperty]
        private bool isEditingNumDays = false;

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

        [RelayCommand]
        private async Task UpdateCustomScheduleAsync()
        {
            if (CustomSchedule is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CustomSchedule>(CustomSchedule))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Custom Schedule.", "OK");
                }
            });
        }

        public async Task LoadWorkoutTemplateCollectionsAsync()
        {
            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == CustomSchedule.WorkoutRoutineId;

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
        private async Task RemoveWorkoutTemplateCollection(int id)
        {
            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemByKeyAsync<WorkoutTemplateCollection>(id))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                }

                await LoadWorkoutTemplateCollectionsAsync();
            });
        }

        [RelayCommand]
        private async Task AddWorkoutTemplateCollectionToDay(WorkoutTemplate workoutTemplate)
        {
            if (workoutTemplate is null) return;

            int scheduleId = CustomSchedule.WorkoutRoutineId;

            await ExecuteAsync(async () =>
            {
                WorkoutTemplateCollection workoutCollection = new()
                {
                    Day = SelectedDay,
                    WorkoutRoutineId = scheduleId,
                    WorkoutTemplateId = workoutTemplate.Id,
                    WorkoutTemplateName = workoutTemplate.Name
                };
                await _context.AddItemAsync<WorkoutTemplateCollection>(workoutCollection);
            });

            await LoadWorkoutTemplateCollectionsAsync();
        }

        [RelayCommand]
        private async Task ShowWorkoutTemplateListSidebar(string dayString)
        {
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

            ShowWorkoutTemplateList = true;

            SelectedDay = day;
        }

        [RelayCommand]
        private void HideWorkoutTemplateList()
        {
            ShowWorkoutTemplateList = false;
        }

        [RelayCommand]
        private static async Task GoToWorkoutTemplate()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutTemplateListPage)}");
        }

        [RelayCommand]
        private void EnableEditNumDays()
        {
            IsEditingNumDays = true;
        }

        [RelayCommand]
        private void DisableEditNumDays()
        {
            IsEditingNumDays = false;
        }

        [RelayCommand]
        private async Task UpdateNumDays()
        {
            if (CustomSchedule is null)
                return;

            if (CustomSchedule.NumDaysInSchedule < 2 || CustomSchedule.NumDaysInSchedule > 14)
            {
                await Shell.Current.DisplayAlert("Error", "Number must be between 2 and 14.", "OK");
                return;
            }

            await UpdateCustomScheduleAsync();
            OnPropertyChanged(nameof(CustomSchedule));
            DisableEditNumDays();
        }
    }
}
