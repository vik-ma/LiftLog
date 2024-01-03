using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Helpers;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(CompletedWorkout), nameof(CompletedWorkout))]
    public partial class StartedWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private CompletedWorkout completedWorkout;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        private readonly List<int> SetListIdOrder = new();

        private readonly List<string> SetPropertyList;

        public StartedWorkoutViewModel(DatabaseContext context)
        {
            _context = context;
            SetPropertyList = SetProperties.SetPropertyList;
        }

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        private ObservableCollection<SetPackage> setList = new();

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

        private void LoadSetListIdOrder()
        {
            if (WorkoutTemplate is null) return;

            if (string.IsNullOrEmpty(WorkoutTemplate.SetListOrder)) return;

            string[] setList = WorkoutTemplate.SetListOrder.Split(',');

            foreach (string s in setList)
            {
                if (int.TryParse(s, out int setId))
                {
                    SetListIdOrder.Add(setId);
                }
            }

            OnPropertyChanged(nameof(WorkoutTemplate));
        }

        public async Task LoadSetListFromWorkoutTemplateIdAsync()
        {
            if (CompletedWorkout is null) return;

            // Load WorkoutTemplate from WorkoutTemplateId
            WorkoutTemplate = await _context.GetItemByKeyAsync<WorkoutTemplate>(CompletedWorkout.WorkoutTemplateId);

            if (WorkoutTemplate is null) return;

            LoadSetListIdOrder();

            SetList.Clear();

            List<SetPackage> setPackageList = new();

            Expression<Func<SetTemplate, bool>> predicateSetTemplate = entity => entity.WorkoutTemplateId == WorkoutTemplate.Id;
            Expression<Func<CompletedSet, bool>> predicateCompletedSet = entity => entity.CompletedWorkoutId == CompletedWorkout.Id;

            try
            {
                var filteredSetTemplateList = await _context.GetFilteredAsync<SetTemplate>(predicateSetTemplate);

                IEnumerable<CompletedSet> filteredCompletedSetList = null;
                if (CompletedWorkout.Id != 0)
                {
                    // Get any CompletedSets if CompletedWorkout has been saved
                    filteredCompletedSetList = await _context.GetFilteredAsync<CompletedSet>(predicateCompletedSet);
                }

                foreach (var item in filteredSetTemplateList)
                {
                    SetPackage setPackage = new() 
                    { 
                        SetTemplate = item,
                        // Set the saved CompletedSet to the corresponding SetTemplate if it exists
                        // Otherwise create a new CompletedSet object
                        CompletedSet = filteredCompletedSetList?.FirstOrDefault(c => c.SetTemplateId == item.Id) ?? new CompletedSet
                        {
                            ExerciseName = item.ExerciseName,
                            CompletedWorkoutId = CompletedWorkout.Id
                        }
                    };

                    setPackageList.Add(setPackage);
                }

                if (setPackageList.Any())
                {
                    // Sort the SetList by its SetListIdOrder
                    SetList = new ObservableCollection<SetPackage>(setPackageList.OrderBy(obj => SetListIdOrder.IndexOf(obj.SetTemplate.Id)));
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                return;
            }
        }

        [RelayCommand]
        private async Task GoToWorkoutTemplateDetails()
        {
            if (WorkoutTemplate is null)
            {
                await Shell.Current.DisplayAlert("Error", "Workout does not exist", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["WorkoutTemplate"] = WorkoutTemplate
            };

            await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?Id={WorkoutTemplate.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task GoToCreateSetTemplatePage(SetTemplate selectedSetTemplate)
        {
            if (selectedSetTemplate is null) return;

            SetWorkoutTemplatePackage package = new()
            {
                WorkoutTemplate = WorkoutTemplate,
                SetTemplate = selectedSetTemplate,
                IsEditing = true
            };

            var navigationParameter = new Dictionary<string, object>
            {
                ["SetWorkoutTemplatePackage"] = package
            };

            await Shell.Current.GoToAsync($"{nameof(CreateSetTemplatePage)}?Id={WorkoutTemplate.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task SaveDate()
        {
            if (CompletedWorkout is null) return;

            string ymdDate = DateTimeHelper.FormatDateTimeToYmdString(SelectedDate);

            CompletedWorkout.Date = ymdDate;

            await UpdateCompletedWorkoutAsync();

            OnPropertyChanged(nameof(CompletedWorkout));
        }

        private async Task UpdateCompletedWorkoutAsync()
        {
            if (CompletedWorkout is null) return;

            if (await _context.ItemExistsByKeyAsync<CompletedWorkout>(CompletedWorkout.Id))
            {
                // Update CompletedWorkout if it is already created
                await ExecuteAsync(async () =>
                {
                    if (!await _context.UpdateItemAsync<CompletedWorkout>(CompletedWorkout))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout.", "OK");
                    }
                });
            }
            // Create new CompletedWorkout if not created
            else await CreateCompletedWorkoutAsync(); 
        }

        private async Task CreateCompletedWorkoutAsync()
        {
            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<CompletedWorkout>(CompletedWorkout);
            });
        }

        [RelayCommand]
        private static void ShowEditSetProperties(SetPackage setPackage)
        {
            if (setPackage is null) return;

            setPackage.IsEditingSetProperties = true;
        }

        [RelayCommand]
        private static void HideEditSetProperties(SetPackage setPackage)
        {
            if (setPackage is null) return;

            setPackage.IsEditingSetProperties = false;
        }

        [RelayCommand]
        private async Task SaveSetAsync(SetPackage setPackage)
        {
            if (setPackage is null) return;

            setPackage.CompletedSet.IsCompleted = true;
            setPackage.CompletedSet.TimeCompleted = DateTimeHelper.GetCurrentFormattedDateTime();
            setPackage.CompletedSet.SetTemplateId = setPackage.SetTemplate.Id;

            await ExecuteAsync(async () =>
            {
                if (!await _context.AddItemAsync<CompletedSet>(setPackage.CompletedSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to save Set.", "OK");
                }
            });

            OnPropertyChanged(nameof(setPackage.CompletedSet));
        }

        [RelayCommand]
        private async Task UpdateSetAsync(CompletedSet completedSet)
        {
            if (completedSet is null) return;

            if (!completedSet.IsCompleted) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CompletedSet>(completedSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when trying to update Set.", "OK");
                }
            });
        }
    }
}
