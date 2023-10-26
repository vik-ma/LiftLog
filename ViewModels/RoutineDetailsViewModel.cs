using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
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
        private Routine routine;

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
                    await Shell.Current.DisplayAlert("Error", "Custom Schedule not implemented yet.", "OK");
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Schedule does not exist!", "OK");
            }

        }
    }
}
