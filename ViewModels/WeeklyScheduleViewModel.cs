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
        private ObservableCollection<WorkoutTemplateCollection> day1WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day2WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day3WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day4WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day5WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day6WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day7WorkoutTemplateCollectionList = new();

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
                LoadWorkoutTemplateCollectionsForEachDay(filteredWtcList);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
            }
        }

        private async void LoadWorkoutTemplateCollectionsForEachDay(IEnumerable<WorkoutTemplateCollection> filteredWtcList)
        {
            foreach (var item in filteredWtcList)
            {
                //Expression<Func<WorkoutTemplate, bool>> predicate = entity => entity.Id == item.WorkoutTemplateId;

                //IEnumerable<WorkoutTemplateCollection> filteredWtList = null;
                try
                {
                    var workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(item.WorkoutTemplateId);
                    item.WorkoutTemplateName = workoutTemplate.Name;
                    //filteredWtList = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workout name.", "OK");
                    return;
                }

                int day = item.Day;

                switch (day)
                {
                    case 0:
                        Day1WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 1:
                        Day2WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 2:
                        Day3WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 3:
                        Day4WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 4:
                        Day5WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 5:
                        Day6WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 6:
                        Day7WorkoutTemplateCollectionList.Add(item);
                        break;

                    default:
                        await Shell.Current.DisplayAlert("Error", "Invalid Day.", "OK");
                        break;
                }
            }
        }

        [RelayCommand]
        private async Task RemoveWorkoutTemplateForDayAsync(object multiBinding)
        {
            await Shell.Current.DisplayAlert("Error", multiBinding.ToString(), "OK");
        }
    }
}
