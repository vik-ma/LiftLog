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

        public async Task LoadNumDaysInSchedule()
        {
            if (WorkoutRoutine is null) return;

            DayNameList.Clear();

            if (WorkoutRoutine.IsScheduleWeekly) 
            {
                DayNameList = new(WeekdayHelper.WeekdayList);
            } 
            else
            {
                await ExecuteAsync(async () =>
                {
                    var schedule = await _context.GetItemByKeyAsync<CustomSchedule>(WorkoutRoutine.ScheduleId);

                    if (schedule is null)
                    {
                        await ResetScheduleId(true);
                        return;
                    } 

                    for (int i = 0; i < WorkoutRoutine.NumDaysInSchedule; i++) 
                    {
                        DayNameList.Add($"Day {i+1}");
                    }
                });
            }
        }

        public async Task LoadWorkoutScheduleList()
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

        private async Task ResetScheduleId(bool scheduleNotFound)
        {
            WorkoutRoutine.ScheduleId = 0;
            WorkoutRoutine.IsScheduleWeekly = false;
            WorkoutRoutine.NonWeeklyScheduleStartDate = null;

            await UpdateWorkoutRoutine();
            await DeleteWorkoutTemplateCollectionsByWorkoutRoutineId();

            WorkoutScheduleList.Clear();
            DayNameList.Clear();

            if (scheduleNotFound) await Shell.Current.DisplayAlert("Schedule Not Found", "Schedule could not be found and has been reset.", "OK");
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
        private async Task UpdateWorkoutRoutine()
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
        private async Task VisitSchedule()
        {
            if (WorkoutRoutine is null) return;

            if (WorkoutRoutine.ScheduleId == 0)
            {
                await CreateNewSchedule();
                return;
            }

            try
            {
                if (WorkoutRoutine.IsScheduleWeekly)
                {
                    WeeklySchedule weeklySchedule;

                    try
                    {
                        weeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(WorkoutRoutine.ScheduleId);

                        if (weeklySchedule is null)
                        {
                            await ResetScheduleId(true);
                            return;
                        }

                        await GoToWeeklySchedulePage(weeklySchedule);
                    }
                    catch
                    {
                        await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Schedule.", "OK");
                        return;
                    }
                } 
                else
                {
                    CustomSchedule customSchedule;

                    try
                    {
                        customSchedule = await _context.GetItemByKeyAsync<CustomSchedule>(WorkoutRoutine.ScheduleId);

                        if (customSchedule is null)
                        {
                            await ResetScheduleId(true);
                            return;
                        }

                        await GoToCustomSchedulePage(customSchedule);
                    }
                    catch
                    {
                        await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Schedule.", "OK");
                        return;
                    }
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Error Loading Schedule!", "OK");
            }
        }

        private async Task CreateNewSchedule()
        {
            string scheduleType = await Shell.Current.DisplayActionSheet("What type of Schedule?", "Cancel", null, "Weekly", "Custom");
        
            if (scheduleType == "Weekly")
            {
                WeeklySchedule weeklySchedule = new() { WorkoutRoutineId = WorkoutRoutine.Id };

                await ExecuteAsync(async () =>
                {
                    if (!await _context.AddItemAsync<WeeklySchedule>(weeklySchedule))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when creating Weekly Schedule.", "OK");
                    }
                    else
                    {
                        WorkoutRoutine.ScheduleId = weeklySchedule.Id;
                        WorkoutRoutine.IsScheduleWeekly = true;
                        await UpdateWorkoutRoutine();
                    }
                });
                
                await GoToWeeklySchedulePage(weeklySchedule);
            }

            if (scheduleType == "Custom")
            {
                string enteredNumber = await Shell.Current.DisplayPromptAsync("Number Of Days In Schedule", "How many days should the schedule contain?\n(Must be between 2 and 14)\n", "OK", "Cancel");

                if (enteredNumber == null) return;

                bool validInput = int.TryParse(enteredNumber, out int numberOfDays);

                if (!validInput || numberOfDays < 2 || numberOfDays > 14)
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid input.", "OK");
                    return;
                }

                CustomSchedule customSchedule = new() 
                { 
                    WorkoutRoutineId = WorkoutRoutine.Id,
                    NumDaysInSchedule = numberOfDays
                };

                await ExecuteAsync(async () =>
                {
                    if (!await _context.AddItemAsync<CustomSchedule>(customSchedule))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when creating Custom Schedule.", "OK");
                    }
                    else
                    {
                        WorkoutRoutine.ScheduleId = customSchedule.Id;
                        WorkoutRoutine.IsScheduleWeekly = false;
                        await UpdateWorkoutRoutine();
                    }
                });

                await GoToCustomSchedulePage(customSchedule);
            }
        }

        private async Task GoToWeeklySchedulePage(WeeklySchedule weeklySchedule)
        {
            if (weeklySchedule is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["WeeklySchedule"] = weeklySchedule
            };

            await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={weeklySchedule.Id}", navigationParameter);

        }

        private async Task GoToCustomSchedulePage(CustomSchedule customSchedule)
        {
            if (customSchedule is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["CustomSchedule"] = customSchedule
            };

            await Shell.Current.GoToAsync($"{nameof(CustomSchedulePage)}?Id={customSchedule.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task DeleteSchedule()
        {
            bool userClickedDelete = await Shell.Current.DisplayAlert("Delete Schedule", $"Are you sure you want to delete Schedule?\nThis can not be undone.", "Delete", "Cancel");

            if (userClickedDelete) await ResetScheduleId(false);
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

            WorkoutRoutine.NonWeeklyScheduleStartDate = DateTimeHelper.FormatDateTimeToYmdString(SelectedDate);

            await UpdateWorkoutRoutine();

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
        private void ConvertScheduleToWeekly()
        {
            if (WorkoutRoutine is null || WorkoutRoutine.IsScheduleWeekly) return;

            WorkoutRoutine.IsScheduleWeekly = true;
            WorkoutRoutine.NumDaysInSchedule = 7;

            OnPropertyChanged(nameof(WorkoutRoutine));
        }

        [RelayCommand]
        private async Task ConvertScheduleToCustom()
        {
            if (WorkoutRoutine is null || WorkoutRoutine.IsScheduleWeekly) return;

            string enteredNumber = await Shell.Current.DisplayPromptAsync("Number Of Days In Schedule", "How many days should the schedule contain?\n(Must be between 2 and 14)\n", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int numberOfDays);

            if (!validInput || numberOfDays < 2 || numberOfDays > 14)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid input.", "OK");
                return;
            }

            WorkoutRoutine.IsScheduleWeekly = false;
            WorkoutRoutine.NumDaysInSchedule = numberOfDays;

            OnPropertyChanged(nameof(WorkoutRoutine));
        }
    }
}
