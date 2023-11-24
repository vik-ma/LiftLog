using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using LocalLiftLog.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq.Expressions;

namespace LocalLiftLog.ViewModels
{
    public partial class TimePeriodViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public TimePeriodViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<TimePeriod> timePeriodList = new();

        public async Task LoadTimePeriodsAsync()
        {
            await ExecuteAsync(async () =>
            {
                TimePeriodList.Clear();

                var timePeriods = await _context.GetAllAsync<TimePeriod>();

                if (timePeriods is not null && timePeriods.Any())
                {
                    timePeriods ??= new ObservableCollection<TimePeriod>();

                    foreach (var period in timePeriods)
                    {
                        TimePeriodList.Add(period);
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
        private async Task CreateTimePeriodAsync()
        {
            await ExecuteAsync(async () =>
            {
                TimePeriod period = new();
                await _context.AddItemAsync<TimePeriod>(period);
                TimePeriodList.Add(period);
            });
        }

        [RelayCommand]
        private async Task UpdateTimePeriodAsync(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<TimePeriod>(timePeriod))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Time Period.", "OK");
                }

                await LoadTimePeriodsAsync();
            });
        }

        [RelayCommand]
        private async Task DeleteTimePeriodAsync(int id)
        {
            TimePeriod timePeriod = TimePeriodList.FirstOrDefault(p => p.Id == id);

            if (timePeriod is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<TimePeriod>(timePeriod))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Time Period.", "OK");
                }
            });

            await LoadTimePeriodsAsync();
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
