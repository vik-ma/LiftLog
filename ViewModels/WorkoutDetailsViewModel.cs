namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutTemplate), nameof(WorkoutTemplate))]
    public partial class WorkoutDetailsViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        private List<int> SetListIdOrder = new();

        public WorkoutDetailsViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<SetExercisePackage> setList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> filteredWorkoutTemplateList = new();

        [ObservableProperty]
        private bool workoutTemplateContainsInvalidExercise = false;

        private WorkoutTemplateListPopupPage Popup;

        [ObservableProperty]
        private string popupTitle = "Copy Workout";

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

            string[] setList = WorkoutTemplate.SetListOrder.Split(",");

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
            if (WorkoutTemplate is null) return;

            LoadSetListIdOrder();

            SetList.Clear();

            WorkoutTemplateContainsInvalidExercise = false;

            List<SetExercisePackage> setTemplateExercisePackageList = new();

            Expression<Func<Set, bool>> predicate = entity => entity.WorkoutTemplateId == WorkoutTemplate.Id && entity.IsTemplate == true;

            try
            {
                var filteredSetList = await _context.GetFilteredAsync<Set>(predicate);

                foreach (var item in filteredSetList)
                {
                    Exercise exercise = await _context.GetItemByKeyAsync<Exercise>(item.ExerciseId);

                    if (exercise is null) WorkoutTemplateContainsInvalidExercise = true;

                    SetExercisePackage setTemplateExercisePackage = new()
                    {
                        Set = item,
                        Exercise = exercise ?? new() { Name = "Invalid Exercise" }
                    };

                    setTemplateExercisePackageList.Add(setTemplateExercisePackage);
                }

                if (setTemplateExercisePackageList.Any())
                {
                    // Sort the SetList by its SetListIdOrder
                    SetList = new ObservableCollection<SetExercisePackage>(setTemplateExercisePackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.Set.Id)));
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Sets.", "OK");
                return;
            }
        }

        public async Task LoadWorkoutTemplatesAsync()
        {
            if (WorkoutTemplate is null) return;

            await ExecuteAsync(async () =>
            {
                WorkoutTemplateList.Clear();

                var workoutTemplates = await _context.GetAllAsync<WorkoutTemplate>();

                if (workoutTemplates is not null && workoutTemplates.Any())
                {
                    workoutTemplates ??= new ObservableCollection<WorkoutTemplate>();

                    foreach (var workout in workoutTemplates)
                    {
                        // Don't add current WorkoutTemplate to List
                        if (workout.Id == WorkoutTemplate.Id) continue;

                        WorkoutTemplateList.Add(workout);
                    }
                }

                FilteredWorkoutTemplateList = new(WorkoutTemplateList);
            });
        }

        private async Task CreateWorkoutTemplateAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (!await _context.AddItemAsync<WorkoutTemplate>(WorkoutTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when saving Workout Template.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task SaveWorkoutTemplateAsync()
        {
            if (WorkoutTemplate is null) return;

            var (isValid, errorMessage) = WorkoutTemplate.Validate();

            if (!isValid ) 
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                return;
            }

            if (WorkoutTemplate.Id == 0)
            {
                // Create new WorkoutTemplate if it does not exist
                await CreateWorkoutTemplateAsync();
            }
            else
            {
                // Update WorkoutTemplate it has already been created
                await UpdateWorkoutTemplateAsync();
            }

            await GoBack();
        }

        [RelayCommand]
        private async Task UpdateWorkoutTemplateAsync()
        {
            if (WorkoutTemplate is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WorkoutTemplate>(WorkoutTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout Template.", "OK");
                }
            });
        }

        [RelayCommand]
        private async Task ShowWorkoutTemplateListPopup()
        {
            if (WorkoutTemplate is null) return;

            await LoadWorkoutTemplatesAsync();

            WorkoutTemplateListPopupPageHandler handler = new()
            {
                ViewModelType = "WorkoutDetails",
                WorkoutDetailsViewModel = this
            };
            Popup = new WorkoutTemplateListPopupPage(handler);
            await Shell.Current.ShowPopupAsync(Popup);
        }

        [RelayCommand]
        public void ClosePopup()
        {
            if (Popup is null) return;

            Popup.Close();
        }

        public async Task CopyWorkout(int existingWorkoutId)
        {
            if (WorkoutTemplate is null) return;

            WorkoutTemplate existingWorkoutTemplate = WorkoutTemplateList.Where(p => p.Id == existingWorkoutId).FirstOrDefault();

            if (existingWorkoutTemplate is null) return;

            if (SetList.Any())
            {
                await Shell.Current.DisplayAlert("Error", "Can only copy workout if Set List is empty!", "OK");
                return;
            }

            if (existingWorkoutId == WorkoutTemplate.Id)
            {
                await Shell.Current.DisplayAlert("Error", "Can not copy the same workout!", "OK");
                return;
            }

            Expression<Func<Set, bool>> predicate = entity => entity.WorkoutTemplateId == existingWorkoutId;

            IEnumerable<Set> filteredList = null;
            try
            {
                // Get all Set Templates of old WorkoutTemplate Id
                filteredList = await _context.GetFilteredAsync<Set>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Templates.", "OK");
            }

            if (!filteredList.Any())
            {
                await Shell.Current.DisplayAlert("Error", "Workout Set List is empty!", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(WorkoutTemplate.Name))
            {
                // Copy existing Workout Template Name if no Name is set
                WorkoutTemplate.Name = $"{existingWorkoutTemplate.Name} (Copy)";
            }

            if (WorkoutTemplate.Id == 0)
            {
                // Save current Workout Template if it does not exist
                await CreateWorkoutTemplateAsync();
            }

            await ExecuteAsync(async () =>
            {
                WorkoutTemplate.SetListOrder = existingWorkoutTemplate.SetListOrder;

                LoadSetListIdOrder();

                // Order the sets for the new workout the same as the old one
                var orderedFilteredList = filteredList.OrderBy(obj => SetListIdOrder.IndexOf(obj.Id));

                // Copy old Set Templates but with new WorkoutTemplateId
                foreach (var item in orderedFilteredList)
                {
                    var itemCopy = item.Clone();
                    itemCopy.WorkoutTemplateId = WorkoutTemplate.Id;
                    await _context.AddItemAsync<Set>(itemCopy);
                }
            });

            await LoadSetListFromWorkoutTemplateIdAsync();

            await GenerateSetListOrderString();

            OnPropertyChanged(nameof(WorkoutTemplate));
            OnPropertyChanged(nameof(SetList));
        }

        [RelayCommand]
        private async Task GoToCreateSetPage(SetWorkoutTemplatePackage package)
        {
            if (package is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["SetWorkoutTemplatePackage"] = package
            };

            await Shell.Current.GoToAsync($"{nameof(CreateSetTemplatePage)}?Id={WorkoutTemplate.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task DeleteSetAsync(Set set)
        {
            if (set is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<Set>(set))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set.", "OK");
                }
            });

            await LoadSetListFromWorkoutTemplateIdAsync();

            await GenerateSetListOrderString();
        }

        [RelayCommand]
        private async Task CreateNewSetTemplate()
        {
            if (WorkoutTemplate is null) return;

            if (string.IsNullOrWhiteSpace(WorkoutTemplate.Name))
            {
                // Set default name for Workout Template if one is not set
                WorkoutTemplate.Name = "New Workout Template";
            }

            if (WorkoutTemplate.Id == 0)
            {
                // Create new WorkoutTemplate if it does not exist
                await CreateWorkoutTemplateAsync();
            }

            SetWorkoutTemplatePackage package = new()
            {
                WorkoutTemplate = WorkoutTemplate,
                Set = new(),
                IsEditing = false
            };

            await GoToCreateSetPage(package);
        }

        [RelayCommand]
        private async Task EditSetAsync(Set set)
        {
            if (set is null) return;

            SetWorkoutTemplatePackage package = new()
            {
                WorkoutTemplate = WorkoutTemplate,
                Set = set,
                IsEditing = true
            };

            await GoToCreateSetPage(package);
        }

        public async Task GenerateSetListOrderString()
        {
            if (WorkoutTemplate is null) return;

            IEnumerable<string> setIdList = SetList.Select(set => set.Set.Id.ToString());

            WorkoutTemplate.SetListOrder = string.Join(",", setIdList);

            await UpdateWorkoutTemplateAsync();
        }

        [RelayCommand]
        private async Task MoveSetUp(Set set)
        {
            if (set is null) return;

            // Get SetList index of current Set
            int setIndex = SetList.ToList().FindIndex(item => item.Set.Equals(set));

            // Do nothing if item is already first in list
            if (setIndex < 1) return;

            // Swap current Set with Set at the index before
            (SetList[setIndex - 1], SetList[setIndex]) = (SetList[setIndex], SetList[setIndex - 1]);
            
            await GenerateSetListOrderString();
        }

        [RelayCommand]
        private async Task MoveSetDown(Set set)
        {
            if (set is null) return;

            // Get SetList index of current Set
            int setIndex = SetList.ToList().FindIndex(item => item.Set.Equals(set));

            // Do nothing if item is already last in list
            if (setIndex > SetList.Count - 2) return;

            // Swap current Set with Set at the index after
            (SetList[setIndex + 1], SetList[setIndex]) = (SetList[setIndex], SetList[setIndex + 1]);

            await GenerateSetListOrderString();
        }

        [RelayCommand]
        private async Task DeleteAllSets()
        {
            if (WorkoutTemplate is null) return;

            bool userClickedDelete = await Shell.Current.DisplayAlert("Remove All Sets", $"Are you sure you want to delete all sets from the Workout?\nThis can not be undone.", "Delete", "Cancel");

            if (!userClickedDelete) return;

            try 
            {
                foreach (var setPackage in SetList)
                {
                    await _context.DeleteItemAsync<Set>(setPackage.Set);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set.", "OK");
            }
            finally
            {
                SetList.Clear();
            }
            
            WorkoutTemplate.SetListOrder = "";
            SetListIdOrder = new();

            await UpdateWorkoutTemplateAsync();

            OnPropertyChanged(nameof(WorkoutTemplate));
            OnPropertyChanged(nameof(SetList));
        }
    }
}