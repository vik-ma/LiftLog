using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using Microsoft.Maui.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private int day1WorkoutTemplateCollectionId;

        [ObservableProperty]
        private int day2WorkoutTemplateCollectionId;

        [ObservableProperty]
        private int day3WorkoutTemplateCollectionId;

        [ObservableProperty]
        private int day4WorkoutTemplateCollectionId;

        [ObservableProperty]
        private int day5WorkoutTemplateCollectionId;

        [ObservableProperty]
        private int day6WorkoutTemplateCollectionId;

        [ObservableProperty]
        private int day7WorkoutTemplateCollectionId;

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

        private readonly string[] daysOfWeekList =
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
        };

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
            int[] workoutDayIdList =
            {
                WeeklySchedule.Day1WorkoutTemplateCollectionId,
                WeeklySchedule.Day2WorkoutTemplateCollectionId,
                WeeklySchedule.Day3WorkoutTemplateCollectionId,
                WeeklySchedule.Day4WorkoutTemplateCollectionId,
                WeeklySchedule.Day5WorkoutTemplateCollectionId,
                WeeklySchedule.Day6WorkoutTemplateCollectionId,
                WeeklySchedule.Day7WorkoutTemplateCollectionId
            };

            List<WorkoutTemplateCollection> workoutCollectionList = Enumerable.Repeat(new WorkoutTemplateCollection(), 7).ToList();

            for (int i = 0; i < 7; i++)
            {
                WorkoutTemplateCollection workoutTemplateCollectionObj;

                if (workoutDayIdList[i] == 0) workoutTemplateCollectionObj = null;
                else 
                {
                    try
                    {
                        workoutTemplateCollectionObj = await _context.GetItemByKeyAsync<WorkoutTemplateCollection>(workoutDayIdList[i]);
                    }
                    catch
                    {
                        bool userClickedNo = await Shell.Current.DisplayAlert("Workout Not Found", $"The Workout for {daysOfWeekList[i]} was not found. Remove Workout from {daysOfWeekList[i]}?", "No", "Yes");
                        
                        // Reset value for day if user clicked Yes
                        if (!userClickedNo) ResetWorkoutIdValue(i);

                        workoutTemplateCollectionObj = null;
                    }
                }
                
                workoutCollectionList[i] = workoutTemplateCollectionObj;
            }
        }

        private async void ResetWorkoutIdValue(int day)
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
                    await Shell.Current.DisplayAlert("Error", "Invalid Day.", "OK");
                    break;
            }

            updateWeeklyScheduleCommand.Execute(WeeklySchedule);
            OnPropertyChanged(nameof(WeeklySchedule));
        }
    }
}
