﻿namespace LocalLiftLog.ViewModels
{
    public partial class TimePeriodViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        public TimePeriodViewModel(DatabaseContext context, UserPreferencesViewModel userSettings)
        {
            _context = context;
            userSettingsViewModel = userSettings;
        }

        [ObservableProperty]
        private ObservableCollection<TimePeriod> timePeriodList = new();

        [ObservableProperty]
        private DateTime selectedDate;

        public async Task LoadTimePeriodsAsync()
        {
            await ExecuteAsync(async () =>
            {
                TimePeriodList.Clear();

                var timePeriods = await _context.GetAllAsync<TimePeriod>();

                if (timePeriods is not null && timePeriods.Any())
                {
                    timePeriods ??= new ObservableCollection<TimePeriod>();

                    foreach (var period in timePeriods)
                    {
                        TimePeriodList.Add(period);
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
        private async Task CreateTimePeriodAsync()
        {
            await ExecuteAsync(async () =>
            {
                TimePeriod period = new();
                await _context.AddItemAsync<TimePeriod>(period);
                TimePeriodList.Add(period);
            });
        }

        [RelayCommand]
        private async Task UpdateTimePeriodAsync(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null)
                return;

            await UpdateTimePeriodObjectAsync(timePeriod);
        }

        [RelayCommand]
        private async Task DeleteTimePeriodAsync(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<TimePeriod>(timePeriod))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Time Period.", "OK");
                }
            });

            await LoadTimePeriodsAsync();
        }

        [RelayCommand]
        private async Task SetTimePeriodStartDate(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null)
                return;

            string selectedDateTimeString = DateTimeHelper.FormatDateTimeToYmdString(SelectedDate);

            timePeriod.IsPeriodOngoing = true;
            timePeriod.StartDate = selectedDateTimeString;
            timePeriod.EndDate = null;

            await UpdateTimePeriodObjectAsync(timePeriod);
        }

        [RelayCommand]
        private async Task SetTimePeriodEndDate(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null)
                return;

            if (timePeriod.StartDate is null)
            {
                await Shell.Current.DisplayAlert("Error", "Must set a Start Date before setting End Date.", "OK");
                return;
            }

            string selectedDateTimeString = DateTimeHelper.FormatDateTimeToYmdString(SelectedDate);

            if (DateTimeHelper.ValidateStartAndEndDate(timePeriod.StartDate, selectedDateTimeString))
            {
                timePeriod.EndDate = selectedDateTimeString;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Start and/or End Date.", "OK");
                return;
            }

            timePeriod.IsPeriodOngoing = false;

            await UpdateTimePeriodObjectAsync(timePeriod);
        }

        [RelayCommand]
        private async Task ResetStartDate(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null) return;

            if (timePeriod.StartDate is null) return;

            if (timePeriod.EndDate is not null) 
            {
                bool userClickedReset = await Shell.Current.DisplayAlert("Reset Dates", "To reset Start Date, the End Date must also be reset.\nContinue?", "Reset", "Cancel");

                if (!userClickedReset) return;

                timePeriod.EndDate = null;
            }

            timePeriod.IsPeriodOngoing = false;
            timePeriod.StartDate = null;

            await UpdateTimePeriodObjectAsync(timePeriod);
        }

        [RelayCommand]
        private async Task ResetEndDate(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null) return;

            if (timePeriod.EndDate is null) return;

            timePeriod.EndDate = null;

            await UpdateTimePeriodObjectAsync(timePeriod);
        }

        private async Task UpdateTimePeriodObjectAsync(TimePeriod timePeriod)
        {
            if (timePeriod is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<TimePeriod>(timePeriod))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Time Period.", "OK");
                }

                await LoadTimePeriodsAsync();
            });
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task SetActiveTimePeriod(int id)
        {
            var TimePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (TimePeriod is null) return;

            UserSettingsViewModel.UserSettings.ActiveTimePeriodId = id;

            await UserSettingsViewModel.UpdateUserPreferencesAsync();
        }
    }
}
