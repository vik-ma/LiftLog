﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        private ObservableCollection<ScheduleFactory> _scheduleFactoryList = new();

        public async Task LoadSchedulesAsync()
        {
            await ExecuteAsync(async () =>
            {
                ScheduleFactoryList.Clear();

                var schedules = await _context.GetAllAsync<ScheduleFactory>();

                if (schedules is not null && schedules.Any())
                {
                    schedules ??= new ObservableCollection<ScheduleFactory>();

                    foreach (var schedule in schedules)
                    {
                        ScheduleFactoryList.Add(schedule);
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

                ScheduleFactory schedule = new()
                {
                    ScheduleId = weeklySchedule.Id,
                    IsScheduleWeekly = true
                };

                await _context.AddItemAsync<ScheduleFactory>(schedule);

                weeklySchedule.ScheduleFactoryId = schedule.Id;
                await _context.UpdateItemAsync<WeeklySchedule>(weeklySchedule);

                ScheduleFactoryList.Add(schedule);
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
                CustomSchedule customSchedule = new();
                await _context.AddItemAsync<CustomSchedule>(customSchedule);

                ScheduleFactory schedule = new()
                {
                    ScheduleId = customSchedule.Id,
                    IsScheduleWeekly = false
                };

                await _context.AddItemAsync<ScheduleFactory>(schedule);

                customSchedule.ScheduleFactoryId = schedule.Id;
                customSchedule.NumDaysInSchedule = numberOfDays;
                await _context.UpdateItemAsync<CustomSchedule>(customSchedule);

                ScheduleFactoryList.Add(schedule);
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
        private async Task DeleteScheduleAsync(int id)
        {
            var schedule = ScheduleFactoryList.FirstOrDefault(p => p.Id == id);

            if (schedule is null)
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist.", "OK");
                return;
            };

            await ExecuteAsync(async () =>
            {
                if (schedule.IsScheduleWeekly)
                {
                    if (!await _context.DeleteItemByKeyAsync<WeeklySchedule>(schedule.ScheduleId))
                    {
                        await Shell.Current.DisplayAlert("Delete Error", "Weekly Schedule was not deleted.", "OK");
                    }
                }
                else
                {
                    if (!await _context.DeleteItemByKeyAsync<CustomSchedule>(schedule.ScheduleId))
                    {
                        await Shell.Current.DisplayAlert("Delete Error", "Custom Schedule was not deleted.", "OK");
                    }
                }


                if (await _context.DeleteItemByKeyAsync<ScheduleFactory>(id))
                {
                    ScheduleFactoryList.Remove(schedule);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Schedule was not deleted.", "OK");
                }
            });
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task ViewSchedule(int id)
        {
            var schedule = ScheduleFactoryList.FirstOrDefault(p => p.Id == id);

            if (schedule is null)
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist", "OK");
                return;
            }

            if (schedule.IsScheduleWeekly)
            {
                WeeklySchedule weeklySchedule;

                try
                {
                    weeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(schedule.ScheduleId);
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
            else
            {
                CustomSchedule customSchedule;

                try
                {
                    customSchedule = await _context.GetItemByKeyAsync<CustomSchedule>(schedule.ScheduleId);
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
}
