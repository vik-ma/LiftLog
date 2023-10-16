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
    public partial class WeeklyScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private Dictionary<int, string> daysOfWeek;

        public WeeklyScheduleViewModel(DatabaseContext context)
        {
            _context = context;

            DaysOfWeek = new Dictionary<int, string>
                {
                    { 1, "Monday" },
                    { 2, "Tuesday" },
                    { 3, "Wednesday" },
                    { 4, "Thursday" },
                    { 5, "Friday" },
                    { 6, "Saturday" },
                    { 7, "Sunday" }
                };
        }

        [ObservableProperty]
        private ObservableCollection<WeeklySchedule> _scheduleList = new();

        [ObservableProperty]
        private Routine _operatingSchedule = new();

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
    }
}
