using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using LocalLiftLog.Helpers;
using System.Collections.ObjectModel;

namespace LocalLiftLog.ViewModels
{
    public partial class UserPreferencesViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferences userSettings;

        [ObservableProperty]
        private UserWeight userWeight;

        public UserPreferencesViewModel(DatabaseContext context)
        {
            _context = context;
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
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task InitializeUserPreferencesAsync()
        {
            await LoadUserPreferencesAsync();
            await LoadActiveUserWeightAsync();
        }

        public async Task LoadUserPreferencesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var userPreferences = await _context.GetAllAsync<UserPreferences>();

                if (userPreferences is not null && userPreferences.Any())
                {
                    UserSettings = userPreferences.FirstOrDefault();
                }
                else
                {
                    await CreateUserPreferencesAsync();
                }
            });
        }

        public async Task LoadActiveUserWeightAsync()
        {
            if (UserSettings is null) return;

            if (UserSettings.ActiveUserWeightId == 0) return;

            await ExecuteAsync(async () =>
            {
                UserWeight userWeight = await _context.GetItemByKeyAsync<UserWeight>(UserSettings.ActiveUserWeightId);
                UserWeight = userWeight;
            });
        }

        private async Task CreateUserPreferencesAsync()
        {
            await ExecuteAsync(async () =>
            {
                UserPreferences userPreferences = new();
                await _context.AddItemAsync<UserPreferences>(userPreferences);
                UserSettings = userPreferences;
            });
        }

        [RelayCommand]
        public async Task UpdateUserPreferencesAsync()
        {
            if (UserSettings is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<UserPreferences>(UserSettings))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating User Preferences.", "OK");
                }
            });

            OnPropertyChanged(nameof(UserSettings));
        }

        public async Task CreateUserWeightAsync(UserWeight userWeight)
        {
            if (userWeight is null) return;

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<UserWeight>(userWeight);
                UserSettings.ActiveUserWeightId = userWeight.Id;
                await UpdateUserPreferencesAsync();
            });
        }

        [RelayCommand]
        private async Task ToggleIsUsingMetricUnits()
        {
            if (UserSettings is null) return;

            UserSettings.IsUsingMetricUnits = !UserSettings.IsUsingMetricUnits;

            await UpdateUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task ToggleIsUsing24HourClock()
        {
            if (UserSettings is null) return;

            UserSettings.IsUsing24HourClock = !UserSettings.IsUsing24HourClock;

            await UpdateUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task ToggleShowCompletedSetTimestamp()
        {
            if (UserSettings is null) return;

            UserSettings.ShowCompletedSetTimestamp = !UserSettings.ShowCompletedSetTimestamp;

            await UpdateUserPreferencesAsync();
        }

        //[RelayCommand]
        //private async Task DeleteUserPreferencesAsync()
        //{
        //    if (UserSettings is null) return;

        //    await ExecuteAsync(async () =>
        //    {
        //        if (!await _context.DeleteItemAsync<UserPreferences>(UserSettings))
        //        {
        //            await Shell.Current.DisplayAlert("Error", "Error occured when updating User Preferences.", "OK");
        //        }
        //    });

        //    UserSettings = null;

        //    OnPropertyChanged(nameof(UserSettings));
        //}

        [RelayCommand]
        private async Task ResetUserPreferencesAsync()
        {
            if (UserSettings is null) return;

            UserSettings.ResetUserPreferences();

            await UpdateUserPreferencesAsync();
        }
    }
}
