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
        private async Task UpdateUserPreferencesAsync()
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

        [RelayCommand]
        private async Task ToggleIsUsingMetricUnits()
        {
            if (UserSettings is null) return;

            UserSettings.IsUsingMetricUnits = !UserSettings.IsUsingMetricUnits;

            await UpdateUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task ToggleShowCompletedSetTimestamp()
        {
            if (UserSettings is null) return;

            UserSettings.ShowCompletedSetTimestamp = !UserSettings.ShowCompletedSetTimestamp;

            await UpdateUserPreferencesAsync();
        }
    }
}
