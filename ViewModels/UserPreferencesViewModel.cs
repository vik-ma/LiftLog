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

        public UserPreferencesViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<UserPreferences> userPreferencesList = new();

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
                UserPreferencesList.Clear();

                var userPreferences = await _context.GetAllAsync<UserPreferences>();

                if (userPreferences is not null && userPreferences.Any())
                {
                    userPreferences ??= new ObservableCollection<UserPreferences>();

                    foreach (var userPreference in userPreferences)
                    {
                        UserPreferencesList.Add(userPreference);
                    }
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
                UserPreferencesList.Add(userPreferences);
            });
        }
    }
}
