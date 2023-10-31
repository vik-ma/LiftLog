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

                //int[] workoutDayIdList =
                //{
                //    WeeklySchedule.Day1WorkoutTemplateCollectionId,
                //    WeeklySchedule.Day2WorkoutTemplateCollectionId,
                //    WeeklySchedule.Day3WorkoutTemplateCollectionId,
                //    WeeklySchedule.Day4WorkoutTemplateCollectionId,
                //    WeeklySchedule.Day5WorkoutTemplateCollectionId,
                //    WeeklySchedule.Day6WorkoutTemplateCollectionId,
                //    WeeklySchedule.Day7WorkoutTemplateCollectionId
                //};

                //List<WorkoutTemplateCollection> workoutCollectionList = Enumerable.Repeat(new WorkoutTemplateCollection(), 7).ToList();

                //for (int i = 0; i < 7; i++)
                //{
                //    WorkoutTemplateCollection workoutTemplateCollectionObj;

                //    if (workoutDayIdList[i] == 0) workoutTemplateCollectionObj = null;
                //    else 
                //    {
                //        try
                //        {
                //            workoutTemplateCollectionObj = await _context.GetItemByKeyAsync<WorkoutTemplateCollection>(workoutDayIdList[i]);

                //            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.ScheduleFactoryId == WeeklySchedule.ScheduleFactoryId;

                //            var asd = await _context.GetFilteredAsync<WorkoutTemplateCollection>(predicate);

                //            await Shell.Current.DisplayAlert("Error", asd.ToString(), "OK");
                //        }
                //        catch
                //        {
                //            bool userClickedNo = await Shell.Current.DisplayAlert("Workout Not Found", $"The Workout for {daysOfWeekList[i]} was not found. Remove Workout from {daysOfWeekList[i]}?", "No", "Yes");

                //            // Reset value for day if user clicked Yes
                //            if (!userClickedNo) ResetWorkoutIdValue(i);

                //            workoutTemplateCollectionObj = null;
                //        }
                //    }

                //    workoutCollectionList[i] = workoutTemplateCollectionObj;
                //}
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

        private void ResetWorkoutIdValue(int day)
        {
            switch (day)
            {
                case 0:
                    WeeklySchedule.Day1WorkoutTemplateCollectionId = 0;
                    break;

                case 1:
                    WeeklySchedule.Day2WorkoutTemplateCollectionId = 0;
                    break;

                case 2:
                    WeeklySchedule.Day3WorkoutTemplateCollectionId = 0;
                    break;

                case 3:
                    WeeklySchedule.Day4WorkoutTemplateCollectionId = 0;
                    break;

                case 4:
                    WeeklySchedule.Day5WorkoutTemplateCollectionId = 0;
                    break;

                case 5:
                    WeeklySchedule.Day6WorkoutTemplateCollectionId = 0;
                    break;

                case 6:
                    WeeklySchedule.Day7WorkoutTemplateCollectionId = 0;
                    break;

                default:
                    Shell.Current.DisplayAlert("Error", "Invalid Day.", "OK");
                    break;
            }

            updateWeeklyScheduleCommand.Execute(WeeklySchedule);
            OnPropertyChanged(nameof(WeeklySchedule));
        }
    }
}
