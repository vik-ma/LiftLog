namespace LocalLiftLog.ViewModels
{
    public partial class WorkoutTemplateViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public WorkoutTemplateViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> _workoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> _workoutTemplateCollectionList = new();

        [ObservableProperty]
        private bool _isCreating = false;

        [RelayCommand]
        private void CancelCreating()
        {
            IsCreating = false;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task LoadWorkoutTemplatesAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutTemplateList.Clear();

                var workouts = await _context.GetAllAsync<WorkoutTemplate>();

                if (workouts is not null && workouts.Any())
                {
                    workouts ??= new ObservableCollection<WorkoutTemplate>();

                    foreach (var workout in workouts)
                    {
                        WorkoutTemplateList.Add(workout);
                    }
                }
            });
        }

        public async Task LoadWorkoutTemplateCollectionsAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutTemplateCollectionList.Clear();

                var workoutCollections = await _context.GetAllAsync<WorkoutTemplateCollection>();

                if (workoutCollections is not null && workoutCollections.Any())
                {
                    workoutCollections ??= new ObservableCollection<WorkoutTemplateCollection>();

                    foreach (var workoutCollection in workoutCollections)
                    {
                        WorkoutTemplateCollectionList.Add(workoutCollection);
                    }
                }
            });
        }

        [RelayCommand]
        private async Task CreateWorkoutTemplateAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutTemplate workout = new();
                await _context.AddItemAsync<WorkoutTemplate>(workout);
                WorkoutTemplateList.Add(workout);
            });
        }

        [RelayCommand]
        private async Task CreateWorkoutTemplateCollectionAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutTemplateCollection workoutCollection = new();
                await _context.AddItemAsync<WorkoutTemplateCollection>(workoutCollection);
                WorkoutTemplateCollectionList.Add(workoutCollection);
            });
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
        private async Task UpdateWorkoutTemplateAsync(WorkoutTemplate workoutTemplate)
        {
            if (workoutTemplate is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WorkoutTemplate>(workoutTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout Template.", "OK");
                }

                await LoadWorkoutTemplatesAsync();
            });
        }

        [RelayCommand]
        private async Task UpdateWorkoutTemplateCollectionAsync(WorkoutTemplateCollection workoutTemplateCollection)
        {
            if (workoutTemplateCollection is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WorkoutTemplateCollection>(workoutTemplateCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout Template Collection.", "OK");
                }

                await LoadWorkoutTemplateCollectionsAsync();
            });
        }

        [RelayCommand]
        private async Task DeleteWorkoutTemplateCollectionAsync(WorkoutTemplateCollection workoutTemplateCollection)
        {
            if (workoutTemplateCollection is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<WorkoutTemplateCollection>(workoutTemplateCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                }
            });

            await LoadWorkoutTemplateCollectionsAsync();
        }

        [RelayCommand]
        private async Task DeleteWorkoutTemplateAsync(WorkoutTemplate workoutTemplate)
        {
            if (workoutTemplate is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<WorkoutTemplate>(workoutTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template.", "OK");
                }
            });

            await LoadWorkoutTemplatesAsync();

            await DeleteWorkoutTemplateCollectionsByWorkoutTemplateId(workoutTemplate.Id);

            await DeleteSetsByWorkoutTemplateId(workoutTemplate.Id);
        }

        private async Task DeleteWorkoutTemplateCollectionsByWorkoutTemplateId(int id)
        {
            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutTemplateId == id;

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

            await LoadWorkoutTemplateCollectionsAsync();
        }

        [RelayCommand]
        private async Task GoToWorkoutTemplateDetails(WorkoutTemplate workoutTemplate)
        {
            if (workoutTemplate is null)
            {
                await Shell.Current.DisplayAlert("Error", "Workout does not exist", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutTemplate"] = workoutTemplate
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?Id={workoutTemplate.Id}", navigationParameter);
        }

        private async Task DeleteSetsByWorkoutTemplateId(int id)
        {
            Expression<Func<Set, bool>> predicate = entity => entity.WorkoutTemplateId == id && entity.IsTemplate == true;

            IEnumerable<Set> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<Set>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Sets.", "OK");
            }

            if (!filteredList.Any()) return;

            foreach (var item in filteredList)
            {
                await ExecuteAsync(async () =>
                {
                    if (!await _context.DeleteItemAsync<Set>(item))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when trying to delete Set.", "OK");
                    }
                });
            }
        }
    }
}
