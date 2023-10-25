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
        private ScheduleFactory _operatingSchedule = new();

        [ObservableProperty]
        private ObservableCollection<ScheduleFactory> _scheduleFactoryList = new();

        public async Task LoadSchedulesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var schedules = await _context.GetAllAsync<ScheduleFactory>();

                if (schedules is not null && schedules.Any())
                {
                    ScheduleFactoryList.Clear();

                    schedules ??= new ObservableCollection<ScheduleFactory>();

                    foreach (var schedule in schedules)
                    {
                        ScheduleFactoryList.Add(schedule);
                    }
                }
            });
        }

        [RelayCommand]
        private async Task CreateScheduleAsync()
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
                if (await _context.DeleteItemByKeyAsync<WeeklySchedule>(id))
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
        private async Task ViewWeeklySchedule(int id)
        {
            var schedule = ScheduleFactoryList.FirstOrDefault(p => p.Id == id);

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
            if (OperatingSchedule is null)
                return;

            await ExecuteAsync(async () =>
            {
                // Update WeeklySchedule
                if (await _context.UpdateItemAsync<ScheduleFactory>(OperatingSchedule))
                {
                    var scheduleCopy = OperatingSchedule.Clone();

                    var index = ScheduleFactoryList.IndexOf(OperatingSchedule);
                    ScheduleFactoryList.RemoveAt(index);

                    ScheduleFactoryList.Insert(index, scheduleCopy);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Weekly Schedule.", "OK");
                    return;
                }
            });

            var id = OperatingSchedule.Id;

            OperatingSchedule = await _context.GetItemByKeyAsync<ScheduleFactory>(id);
        }
    }
}
