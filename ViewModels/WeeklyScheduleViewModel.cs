using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

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
        private ObservableCollection<WorkoutTemplate> day1WorkoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> day2WorkoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> day3WorkoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> day4WorkoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> day5WorkoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> day6WorkoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> day7WorkoutTemplateList = new();

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
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
        private async Task UpdateWeeklyScheduleAsync()
        {
            if (WeeklySchedule is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WeeklySchedule>(WeeklySchedule))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Weekly Schedule.", "OK");
                }
            });
        }

        public async Task LoadWorkoutTemplateCollectionsAsync()
        {
            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.ScheduleFactoryId == WeeklySchedule.ScheduleFactoryId;
            
            IEnumerable<WorkoutTemplateCollection> filteredWtcList = null;
            try 
            {
                filteredWtcList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);
                LoadWorkoutTemplateForEachDay(filteredWtcList);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
            }
        }

        private async void LoadWorkoutTemplateForEachDay(IEnumerable<WorkoutTemplateCollection> filteredWtcList)
        {
            foreach (var item in filteredWtcList)
            {
                Expression<Func<WorkoutTemplate, bool>> predicate = entity => entity.Id == item.WorkoutTemplateId;

                IEnumerable<WorkoutTemplate> filteredWtList = null;
                try
                {
                    filteredWtList = await _context.GetFilteredAsync<WorkoutTemplate>(predicate);
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                    return;
                }

                int day = item.Day;

                switch (day)
                {
                    case 0:
                        foreach (var wt in filteredWtList)
                            Day1WorkoutTemplateList.Add(wt);
                        break;

                    case 1:
                        foreach (var wt in filteredWtList)
                            Day2WorkoutTemplateList.Add(wt);
                        break;

                    case 2:
                        foreach (var wt in filteredWtList)
                            Day3WorkoutTemplateList.Add(wt);
                        break;

                    case 3:
                        foreach (var wt in filteredWtList)
                            Day4WorkoutTemplateList.Add(wt);
                        break;

                    case 4:
                        foreach (var wt in filteredWtList)
                            Day5WorkoutTemplateList.Add(wt);
                        break;

                    case 5:
                        foreach (var wt in filteredWtList)
                            Day6WorkoutTemplateList.Add(wt);
                        break;

                    case 6:
                        foreach (var wt in filteredWtList)
                            Day7WorkoutTemplateList.Add(wt);
                        break;

                    default:
                        await Shell.Current.DisplayAlert("Error", "Invalid Day.", "OK");
                        break;
                }
            }
        }
    }
}
