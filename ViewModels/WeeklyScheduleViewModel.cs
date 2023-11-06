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

        [ObservableProperty]
        private bool showWorkoutTemplateList = false;

        [ObservableProperty]
        private int selectedDay;

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

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
            Day1WorkoutTemplateCollectionList.Clear();
            Day2WorkoutTemplateCollectionList.Clear();
            Day3WorkoutTemplateCollectionList.Clear();
            Day4WorkoutTemplateCollectionList.Clear();
            Day5WorkoutTemplateCollectionList.Clear();
            Day6WorkoutTemplateCollectionList.Clear();
            Day7WorkoutTemplateCollectionList.Clear();

            foreach (var item in filteredWtcList)
            {
                WorkoutTemplate workoutTemplate = null;

                await ExecuteAsync(async () =>
                {
                    workoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(item.WorkoutTemplateId);
                });
                
                if (workoutTemplate is null)
                {
                    // Delete WorkoutTemplateCollection if it references a WorkoutTemplate whose Id does not exist
                    await ExecuteAsync(async () =>
                    {
                        if (!await _context.DeleteItemAsync<WorkoutTemplateCollection>(item))
                        {
                            await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                        }
                    });
                    return;
                }

                item.WorkoutTemplateName = workoutTemplate.Name;

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
        private async Task RemoveWorkoutTemplateCollection(int id)
        {
            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemByKeyAsync<WorkoutTemplateCollection>(id))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Workout Template Collection.", "OK");
                }

                await LoadWorkoutTemplateCollectionsAsync();
            });
        }

        [RelayCommand]
        private async Task AddWorkoutTemplateCollectionToDay(int workoutTemplateId)
        {
            int scheduleId = WeeklySchedule.ScheduleFactoryId;

            await ExecuteAsync(async () =>
            {
                WorkoutTemplateCollection workoutCollection = new()
                {
                    Day = SelectedDay,
                    ScheduleFactoryId = scheduleId,
                    WorkoutTemplateId = workoutTemplateId
                };
                await _context.AddItemAsync<WorkoutTemplateCollection>(workoutCollection);
            });

            await LoadWorkoutTemplateCollectionsAsync();
        }

        [RelayCommand]
        private async Task ShowWorkoutTemplateListSidebar(int day)
        {
            ShowWorkoutTemplateList = true;

            if (day < 0 || day > 6)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Day", "OK");
                return;
            }

            SelectedDay = day;
        }

        [RelayCommand]
        private void HideWorkoutTemplateList()
        {
            ShowWorkoutTemplateList = false;
        }

        public async Task LoadWorkoutTemplatesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var workoutTemplates = await _context.GetAllAsync<WorkoutTemplate>();

                if (workoutTemplates is not null && workoutTemplates.Any())
                {
                    WorkoutTemplateList.Clear();

                    workoutTemplates ??= new ObservableCollection<WorkoutTemplate>();

                    foreach (var workout in workoutTemplates)
                    {
                        WorkoutTemplateList.Add(workout);
                    }
                }
            });
        }
    }
}
