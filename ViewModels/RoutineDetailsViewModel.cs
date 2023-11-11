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
    [QueryProperty(nameof(Routine), nameof(Routine))]
    public partial class RoutineDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private RoutineListViewModel _routineListViewModel;

        private readonly DatabaseContext _context;

        public RoutineDetailsViewModel(RoutineListViewModel routineListViewModel, DatabaseContext context)
        {
            _routineListViewModel = routineListViewModel;
            _context = context;
        }

        [ObservableProperty]
        private bool isShowingScheduleList = false;

        [ObservableProperty]
        private Routine routine;

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

        [RelayCommand]
        private async Task UpdateRoutine()
        {
            var id = Routine.Id;

            RoutineListViewModel.SaveRoutineCommand.Execute(RoutineListViewModel.OperatingRoutine);

            Routine = await _context.GetItemByKeyAsync<Routine>(id);
        }

        [RelayCommand]
        private async Task VisitSchedule()
        {
            var ScheduleFactoryId = Routine.ScheduleFactoryId;

            if (ScheduleFactoryId == 0)
            {
                await Shell.Current.DisplayAlert("Error", "No Schedule Is Set!", "OK");
                return;
            }

            try
            {
                var schedule = await _context.GetItemByKeyAsync<ScheduleFactory>(ScheduleFactoryId);

                if (schedule.IsScheduleWeekly)
                {
                    WeeklySchedule weeklySchedule;

                    try
                    {
                        weeklySchedule = await _context.GetItemByKeyAsync<WeeklySchedule>(schedule.ScheduleId);

                        var navigationParameter = new Dictionary<string, object>
                        {
                            ["WeeklySchedule"] = weeklySchedule
                        };

                        await Shell.Current.GoToAsync($"{nameof(WeeklySchedulePage)}?Id={weeklySchedule.Id}", navigationParameter);
                    }
                    catch
                    {
                        await Shell.Current.DisplayAlert("Error", "Weekly Schedule ID does not exist!", "OK");
                        return;
                    }
                } 
                else
                {
                    CustomSchedule customSchedule;

                    try
                    {
                        customSchedule = await _context.GetItemByKeyAsync<CustomSchedule>(schedule.ScheduleId);

                        var navigationParameter = new Dictionary<string, object>
                        {
                            ["CustomSchedule"] = customSchedule
                        };

                        await Shell.Current.GoToAsync($"{nameof(CustomSchedulePage)}?Id={customSchedule.Id}", navigationParameter);
                    }
                    catch
                    {
                        await Shell.Current.DisplayAlert("Error", "Custom Schedule ID does not exist!", "OK");
                        return;
                    }
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist!", "OK");
            }
        }

        [RelayCommand]
        private void ShowScheduleList()
        {
            IsShowingScheduleList = true;
        }
    }
}
