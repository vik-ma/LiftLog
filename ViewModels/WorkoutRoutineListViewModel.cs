namespace LocalLiftLog.ViewModels
{
    public partial class WorkoutRoutineListViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        public WorkoutRoutineListViewModel(DatabaseContext context, UserPreferencesViewModel userSettings)
        {
            _context = context;
            userSettingsViewModel = userSettings;
        }

        [ObservableProperty]
        private ObservableCollection<WorkoutRoutine> _workoutRoutineList = new();

        [ObservableProperty]
        private WorkoutRoutine _operatingWorkoutRoutine = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        [ObservableProperty]
        private bool _isEditing = false;

        public async Task LoadWorkoutRoutinesAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutRoutineList.Clear();

                var WorkoutRoutines = await _context.GetAllAsync<WorkoutRoutine>();

                if (WorkoutRoutines is not null && WorkoutRoutines.Any())
                {
                    WorkoutRoutines ??= new ObservableCollection<WorkoutRoutine>();

                    foreach (var WorkoutRoutine in WorkoutRoutines)
                    {
                        WorkoutRoutineList.Add(WorkoutRoutine);
                    }
                }
            }, "Fetching WorkoutRoutine List...");  
        }

        #nullable enable
        [RelayCommand]
        private void SetOperatingWorkoutRoutine(WorkoutRoutine? WorkoutRoutine)
        {
            OperatingWorkoutRoutine = WorkoutRoutine ?? new();
            IsEditing = true;
        }

        [RelayCommand]
        private void CancelEditing()
        {
            IsEditing = false;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }


        [RelayCommand]
        private async Task SaveWorkoutRoutineAsync()
        {
            if (OperatingWorkoutRoutine is null)
                return;

            var (isValid, errorMessage) = OperatingWorkoutRoutine.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            OperatingWorkoutRoutine.UpdateDateTime();

            var busyText = OperatingWorkoutRoutine.Id == 0 ? "Creating WorkoutRoutine..." : "Updating WorkoutRoutine";

            await ExecuteAsync(async () =>
            {
                if (OperatingWorkoutRoutine.Id == 0)
                {
                    // Create WorkoutRoutine
                    await _context.AddItemAsync<WorkoutRoutine>(OperatingWorkoutRoutine);
                    WorkoutRoutineList.Add(OperatingWorkoutRoutine);

                    int id = WorkoutRoutineList[WorkoutRoutineList.Count - 1].Id;

                    // Send user to WorkoutRoutineDetailsPage of newly created WorkoutRoutine
                    ViewWorkoutRoutineDetailsCommand.Execute(id);
                }
                else
                {
                    // Update WorkoutRoutine
                    if (await _context.UpdateItemAsync<WorkoutRoutine>(OperatingWorkoutRoutine))
                    {
                        var WorkoutRoutineCopy = OperatingWorkoutRoutine.Clone();

                        var index = WorkoutRoutineList.IndexOf(OperatingWorkoutRoutine);
                        WorkoutRoutineList.RemoveAt(index);

                        WorkoutRoutineList.Insert(index, WorkoutRoutineCopy);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when updating WorkoutRoutine.", "OK");
                        return;
                    }
                }
                #nullable disable
                SetOperatingWorkoutRoutineCommand.Execute(new());
            }, busyText);
        }

        [RelayCommand]
        private async Task DeleteWorkoutRoutineAsync(int id)
        {
            var WorkoutRoutine = WorkoutRoutineList.FirstOrDefault(p => p.Id == id);

            if (WorkoutRoutine is null) {
                await Shell.Current.DisplayAlert("Error", "WorkoutRoutine does not exist.", "OK");
                return;
            };

            bool userClickedDelete = await Shell.Current.DisplayAlert("Delete WorkoutRoutine", $"Are you sure you want to delete {WorkoutRoutine.Name}?", "Delete", "Cancel");

            if (!userClickedDelete) return;

            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<WorkoutRoutine>(id))
                {
                    WorkoutRoutineList.Remove(WorkoutRoutine);

                    // Reset ActiveWorkoutRoutineId in UserSettings if current WorkoutRoutine is the same
                    if (id == UserSettingsViewModel.UserSettings.ActiveWorkoutRoutineId)
                    {
                        await UserSettingsViewModel.ResetActiveWorkoutRoutine();
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "WorkoutRoutine was not deleted.", "OK");
                }
            }, "Deleting WorkoutRoutine...");
        }

        #nullable enable
        private async Task ExecuteAsync(Func<Task> operation, string? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText ?? "Processing...";
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
                IsBusy = false;
                BusyText = "Finished";
                IsEditing = false;
            }
        }

        [RelayCommand]
        private async Task ViewWorkoutRoutineDetails(int id)
        {
            var WorkoutRoutine = WorkoutRoutineList.FirstOrDefault(p => p.Id == id);

            if (WorkoutRoutine is null)
            {
                await Shell.Current.DisplayAlert("Error", "WorkoutRoutine does not exist", "OK");
                return;
            }
                
            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutRoutine"] = WorkoutRoutine
            };

            IsEditing = false;

            await Shell.Current.GoToAsync($"{nameof(WorkoutRoutineDetailsPage)}?Id={id}", navigationParameter);
        }

        [RelayCommand]
        private async Task SetActiveWorkoutRoutine(WorkoutRoutine workoutRoutine)
        {
            if (workoutRoutine is null) return;

            UserSettingsViewModel.UserSettings.ActiveWorkoutRoutineId = workoutRoutine.Id;

            await UserSettingsViewModel.UpdateUserPreferencesAsync();

            await UserSettingsViewModel.LoadActiveWorkoutRoutineAsync();
        }
    }
}
