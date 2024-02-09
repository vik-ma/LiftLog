namespace LocalLiftLog.ViewModels
{
    public partial class SetListViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;
        public SetListViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<Set> setList = new();

        [ObservableProperty]
        private ObservableCollection<Set> filteredSetList = new();

        public async Task LoadSetsAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetList.Clear();

                var sets = await _context.GetAllAsync<Set>();

                if (sets is not null && sets.Any())
                {
                    sets ??= new ObservableCollection<Set>();

                    foreach (var set in sets)
                    {
                        SetList.Add(set);
                    }
                }

                FilteredSetList = SetList;
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
        private async Task CreateSetAsync()
        {
            await ExecuteAsync(async () =>
            {
                Set set = new();
                await _context.AddItemAsync<Set>(set);
                SetList.Add(set);
            });
        }

        [RelayCommand]
        private async Task UpdateSetAsync(Set set)
        {
            if (set is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<Set>(set))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Set.", "OK");
                }

                await LoadSetsAsync();
            });
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

            await LoadSetsAsync();
        }

        [RelayCommand]
        private async Task MarkSetAsComplete(Set set)
        {
            if (set is null) return;

            string currentDateTimeString = DateTimeHelper.GetCurrentFormattedDateTime();

            set.IsCompleted = true;
            set.TimeCompleted = currentDateTimeString;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<Set>(set))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Set.", "OK");
                }

                await LoadSetsAsync();
            });
        }

        [RelayCommand]
        private async Task GoToCreateSetTemplatePage(Set set)
        {
            if (set is null || !set.IsTemplate || set.WorkoutTemplateId == 0) return;

            WorkoutTemplate workoutTemplate = null;
            await ExecuteAsync(async () =>
            {
                workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(set.WorkoutTemplateId);
            });

            if (workoutTemplate is null) return;

            SetWorkoutTemplatePackage package = new() 
            {
                WorkoutTemplate = workoutTemplate,
                Set = set,
                IsEditing = true
            };

            var navigationParameter = new Dictionary<string, object>
            {
                ["SetWorkoutTemplatePackage"] = package
            };

            await Shell.Current.GoToAsync($"{nameof(CreateSetTemplatePage)}?Id={workoutTemplate.Id}", navigationParameter);
        }

        [RelayCommand]
        private void FilterSetList(string filterType)
        {
            switch (filterType)
            {
                case "All":
                    FilteredSetList = SetList;
                    break;

                case "Template":
                    FilteredSetList = new(SetList.Where(item => item.IsTemplate));
                    break;

                case "NotTemplate":
                    FilteredSetList = new(SetList.Where(item => !item.IsTemplate));
                    break;

                default:
                    return;
            }
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
