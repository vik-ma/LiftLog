using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    public partial class ScheduleListViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public ScheduleListViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<WeeklySchedule> _weeklyScheduleList = new();

        [ObservableProperty]
        private ObservableCollection<CustomSchedule> _customScheduleList = new();

        public async Task LoadSchedulesAsync()
        {
            await ExecuteAsync(async () =>
            {
                WeeklyScheduleList.Clear();
                CustomScheduleList.Clear();

                var weeklySchedules = await _context.GetAllAsync<WeeklySchedule>();

                if (weeklySchedules is not null && weeklySchedules.Any())
                {
                    weeklySchedules ??= new ObservableCollection<WeeklySchedule>();

                    foreach (var weeklySchedule in weeklySchedules)
                    {
                        WeeklyScheduleList.Add(weeklySchedule);
                    }
                }

                var customSchedules = await _context.GetAllAsync<CustomSchedule>();

                if (customSchedules is not null && customSchedules.Any())
                {
                    customSchedules ??= new ObservableCollection<CustomSchedule>();

                    foreach (var customSchedule in customSchedules)
                    {
                        CustomScheduleList.Add(customSchedule);
                    }
                }
            });
        }

        [RelayCommand]
        private async Task CreateWeeklyScheduleAsync()
        {
            await ExecuteAsync(async () =>
            {
                WeeklySchedule weeklySchedule = new();
                await _context.AddItemAsync<WeeklySchedule>(weeklySchedule);

                WeeklyScheduleList.Add(weeklySchedule);
            });
        }

        [RelayCommand]
        private async Task CreateCustomScheduleAsync()
        {
            string enteredNumber = await Shell.Current.DisplayPromptAsync("Number Of Days In Schedule", "How many days should the schedule contain?\n(Must be between 2 and 14)\n", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int numberOfDays);

            if (!validInput || numberOfDays < 2 || numberOfDays > 14)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid input.", "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                CustomSchedule customSchedule = new() { NumDaysInSchedule = numberOfDays };
                await _context.AddItemAsync<CustomSchedule>(customSchedule);

                CustomScheduleList.Add(customSchedule);
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
        private async Task DeleteWeeklyScheduleAsync(int id)
        {
            var schedule = WeeklyScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemByKeyAsync<WeeklySchedule>(id))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Weekly Schedule.", "OK");
                }
            });

            await LoadSchedulesAsync();
        }

        [RelayCommand]
        private async Task DeleteCustomScheduleAsync(int id)
        {
            var schedule = CustomScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemByKeyAsync<CustomSchedule>(id))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Custom Schedule.", "OK");
                }
            });

            await LoadSchedulesAsync();
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task ViewWeeklySchedule(int id)
        {
            var schedule = WeeklyScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null) return;

            WeeklySchedule weeklySchedule;

            try
            {
                weeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(id);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Weekly Schedule ID does not exist!", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["WeeklySchedule"] = weeklySchedule
            };

            await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={id}", navigationParameter);
        }

        [RelayCommand]
        private async Task ViewCustomSchedule(int id)
        {
            var schedule = CustomScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null) return;

            CustomSchedule customSchedule;

            try
            {
                customSchedule = await _context.GetItemByKeyAsync<CustomSchedule>(id);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Custom Schedule ID does not exist!", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["CustomSchedule"] = customSchedule
            };

            await Shell.Current.GoToAsync($"{nameof(CustomSchedulePage)}?Id={id}", navigationParameter);
        }
    }
}
