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
    [QueryProperty(nameof(WeeklySchedule), nameof(WeeklySchedule))]
    public partial class WeeklyScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WeeklySchedule weeklySchedule;

        public WeeklyScheduleViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<WeeklySchedule> _scheduleList = new();

        public async Task LoadSchedulesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var schedules = await _context.GetAllAsync<WeeklySchedule>();

                if (schedules is not null && schedules.Any())
                {
                    ScheduleList.Clear();

                    schedules ??= new ObservableCollection<WeeklySchedule>();

                    foreach (var routine in schedules)
                    {
                        ScheduleList.Add(routine);
                    }
                }
            });
        }

        [RelayCommand]
        private async Task CreateScheduleAsync()
        {
            await ExecuteAsync(async () =>
            {
                WeeklySchedule schedule = new();
                await _context.AddItemAsync<WeeklySchedule>(schedule);
                ScheduleList.Add(schedule);
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
            var schedule = ScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null)
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist.", "OK");
                return;
            };

            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<WeeklySchedule>(id))
                {
                    ScheduleList.Remove(schedule);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Schedule was not deleted.", "OK");
                }
            });
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task ViewWeeklySchedule(int id)
        {
            var schedule = ScheduleList.FirstOrDefault(p => p.Id == id);

            if (schedule is null)
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["WeeklySchedule"] = schedule
            };

            await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={id}", navigationParameter);
        }

        [RelayCommand]
        private async Task SaveWeeklyScheduleAsync()
        {
            if (WeeklySchedule is null)
                return;

            await ExecuteAsync(async () =>
            {
                // Update WeeklySchedule
                if (await _context.UpdateItemAsync<WeeklySchedule>(WeeklySchedule))
                {
                    var scheduleCopy = WeeklySchedule.Clone();

                    var index = ScheduleList.IndexOf(WeeklySchedule);
                    ScheduleList.RemoveAt(index);

                    ScheduleList.Insert(index, scheduleCopy);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Weekly Schedule.", "OK");
                    return;
                }
            });

            var id = WeeklySchedule.Id;

            WeeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(id);
        }
    }
}
