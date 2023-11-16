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
    [QueryProperty(nameof(Program), nameof(Program))]
    public partial class ProgramDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ProgramListViewModel _programListViewModel;

        private readonly DatabaseContext _context;

        public ProgramDetailsViewModel(ProgramListViewModel programListViewModel, DatabaseContext context)
        {
            _programListViewModel = programListViewModel;
            _context = context;
        }

        [ObservableProperty]
        private bool isShowingScheduleList = false;

        [ObservableProperty]
        private Program program;

        [ObservableProperty]
        private ObservableCollection<ScheduleFactory> _scheduleFactoryList = new();

        [ObservableProperty]
        private ScheduleFactory programSchedule;

        [ObservableProperty]
        private bool isScheduleSet = false;

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
        private async Task UpdateProgram()
        {
            var (isValid, errorMessage) = Program.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            if (!await _context.UpdateItemAsync<Program>(Program))
            {
                await Shell.Current.DisplayAlert("Error", "Error occured when updating Program.", "OK");
                return;
            }

            OnPropertyChanged(nameof(Program));
        }

        [RelayCommand]
        private async Task VisitSchedule()
        {
            var ScheduleFactoryId = Program.ScheduleFactoryId;

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

        public async Task LoadProgramSchedule()
        {
            // Exit function if no ScheduleFactory is set
            if (Program.ScheduleFactoryId == 0) return;

            ScheduleFactory scheduleFactory = null;

            await ExecuteAsync(async () =>
            {
                scheduleFactory = await _context.GetItemByKeyAsync<ScheduleFactory>(Program.ScheduleFactoryId);
            });

            if (scheduleFactory is null)
            {
                // Delete ScheduleFactoryId for Program if ScheduleFactory key does not exist
                Program.ScheduleFactoryId = 0;
                await UpdateProgram();
            }
            else
            {
                // Set the loaded ScheduleFactory as ProgramSchedule
                ProgramSchedule = scheduleFactory;
                IsScheduleSet = true;
            }
        }

        [RelayCommand]
        private async Task SetSchedule(ScheduleFactory schedule)
        {
            if (schedule == null) return;

            Program.ScheduleFactoryId = schedule.Id;

            await UpdateProgram();
            await LoadProgramSchedule();

            IsShowingScheduleList = false;
            IsScheduleSet = true;
        }

        [RelayCommand]
        private async Task GoToScheduleList()
        {
            await Shell.Current.GoToAsync($"{nameof(ScheduleListPage)}");
        }
    }
}
