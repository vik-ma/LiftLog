namespace LocalLiftLog.ViewModels
{
    public partial class CompletedSetViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;
        public CompletedSetViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<Set> setList = new();

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
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
