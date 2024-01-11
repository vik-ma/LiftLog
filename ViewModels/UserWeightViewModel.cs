using CommunityToolkit.Mvvm.ComponentModel;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using LocalLiftLog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace LocalLiftLog.ViewModels
{
    public partial class UserWeightViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public UserWeightViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        public ObservableCollection<UserWeight> userWeightList = new();

        [ObservableProperty]
        public string newWeightInput;

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

        public async Task LoadUserWeightListAsync()
        {
            await ExecuteAsync(async () =>
            {
                UserWeightList.Clear();

                var weightList = await _context.GetAllAsync<UserWeight>();

                if (weightList is not null && weightList.Any())
                {
                    weightList ??= new ObservableCollection<UserWeight>();

                    foreach (var set in weightList)
                    {
                        UserWeightList.Add(set);
                    }
                }
            });
        }

        private async Task CreateUserWeightAsync(UserWeight userWeight)
        {
            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<UserWeight>(userWeight);
                UserWeightList.Add(userWeight);
            });
        }

        [RelayCommand]
        private async Task AddWeight()
        {
            if (string.IsNullOrWhiteSpace(NewWeightInput)) return;

            bool validInput = int.TryParse(NewWeightInput, out int weightInputInt);

            if (!validInput || weightInputInt < ConstantsHelper.BodyWeightInputMinValue || weightInputInt > ConstantsHelper.BodyWeightMaxValue)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Weight Input.\n", "OK");
                return;
            }

            string currentDateTimeString = DateTimeHelper.GetCurrentFormattedDateTime();

            UserWeight userWeight = new()
            {
                BodyWeight = weightInputInt,
                DateTime = currentDateTimeString
            };

            await CreateUserWeightAsync(userWeight);

            await LoadUserWeightListAsync();

            NewWeightInput = "";
        }

        [RelayCommand]
        private async Task DeleteWeightAsync(UserWeight userWeight)
        {
            if (userWeight is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<UserWeight>(userWeight))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Weight.", "OK");
                }
            });

            await LoadUserWeightListAsync();
        }
    }
}
