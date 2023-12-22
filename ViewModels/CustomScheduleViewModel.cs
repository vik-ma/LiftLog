using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(CustomSchedule), nameof(CustomSchedule))]
    public partial class CustomScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private CustomSchedule customSchedule;

        public CustomScheduleViewModel(DatabaseContext context)
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
        private ObservableCollection<WorkoutTemplateCollection> day8WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day9WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day10WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day11WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day12WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day13WorkoutTemplateCollectionList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplateCollection> day14WorkoutTemplateCollectionList = new();
        
        [ObservableProperty]
        private bool showWorkoutTemplateList = false;

        [ObservableProperty]
        private int selectedDay;

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

        [ObservableProperty]
        private bool isEditingNumDays = false;

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
        private async Task UpdateCustomScheduleAsync()
        {
            if (CustomSchedule is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CustomSchedule>(CustomSchedule))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Custom Schedule.", "OK");
                }
            });
        }

        public async Task LoadWorkoutTemplateCollectionsAsync()
        {
            Expression<Func<WorkoutTemplateCollection, bool>> predicate = entity => entity.WorkoutRoutineId == CustomSchedule.WorkoutRoutineId;

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

        public async Task LoadWorkoutTemplatesAsync()
        {
            await ExecuteAsync(async () =>
            {
                WorkoutTemplateList.Clear();

                var workoutTemplates = await _context.GetAllAsync<WorkoutTemplate>();

                if (workoutTemplates is not null && workoutTemplates.Any())
                {
                    workoutTemplates ??= new ObservableCollection<WorkoutTemplate>();

                    foreach (var workout in workoutTemplates)
                    {
                        WorkoutTemplateList.Add(workout);
                    }
                }
            });
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
            Day8WorkoutTemplateCollectionList.Clear();
            Day9WorkoutTemplateCollectionList.Clear();
            Day10WorkoutTemplateCollectionList.Clear();
            Day11WorkoutTemplateCollectionList.Clear();
            Day12WorkoutTemplateCollectionList.Clear();
            Day13WorkoutTemplateCollectionList.Clear();
            Day14WorkoutTemplateCollectionList.Clear();

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

                    case 7:
                        Day8WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 8:
                        Day9WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 9:
                        Day10WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 10:
                        Day11WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 11:
                        Day12WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 12:
                        Day13WorkoutTemplateCollectionList.Add(item);
                        break;

                    case 13:
                        Day14WorkoutTemplateCollectionList.Add(item);
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
        private async Task AddWorkoutTemplateCollectionToDay(WorkoutTemplate workoutTemplate)
        {
            if (workoutTemplate is null) return;

            int scheduleId = CustomSchedule.WorkoutRoutineId;

            await ExecuteAsync(async () =>
            {
                WorkoutTemplateCollection workoutCollection = new()
                {
                    Day = SelectedDay,
                    WorkoutRoutineId = scheduleId,
                    WorkoutTemplateId = workoutTemplate.Id,
                    WorkoutTemplateName = workoutTemplate.Name
                };
                await _context.AddItemAsync<WorkoutTemplateCollection>(workoutCollection);
            });

            await LoadWorkoutTemplateCollectionsAsync();
        }

        [RelayCommand]
        private async Task ShowWorkoutTemplateListSidebar(string dayString)
        {
            int day;

            if (int.TryParse(dayString, out int intValue))
            {
                day = intValue;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Day String", "OK");
                return;
            }

            if (day < 0 || day > 13)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Day", "OK");
                return;
            }

            ShowWorkoutTemplateList = true;

            SelectedDay = day;
        }

        [RelayCommand]
        private void HideWorkoutTemplateList()
        {
            ShowWorkoutTemplateList = false;
        }

        [RelayCommand]
        private static async Task GoToWorkoutTemplate()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutTemplateListPage)}");
        }

        [RelayCommand]
        private void EnableEditNumDays()
        {
            IsEditingNumDays = true;
        }

        [RelayCommand]
        private void DisableEditNumDays()
        {
            IsEditingNumDays = false;
        }

        [RelayCommand]
        private async Task UpdateNumDays()
        {
            if (CustomSchedule is null)
                return;

            if (CustomSchedule.NumDaysInSchedule < 2 || CustomSchedule.NumDaysInSchedule > 14)
            {
                await Shell.Current.DisplayAlert("Error", "Number must be between 2 and 14.", "OK");
                return;
            }

            await UpdateCustomScheduleAsync();
            OnPropertyChanged(nameof(CustomSchedule));
            DisableEditNumDays();
        }
    }
}
