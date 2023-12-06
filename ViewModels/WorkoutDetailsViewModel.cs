using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
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
    [QueryProperty(nameof(WorkoutTemplate), nameof(WorkoutTemplate))]
    public partial class WorkoutDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private WorkoutTemplateViewModel _workoutTemplateViewModel;

        private readonly DatabaseContext _context;

        private readonly ExerciseDataManager _exerciseData;

        private readonly Dictionary<int, string> exerciseGroupDict;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        public WorkoutDetailsViewModel(WorkoutTemplateViewModel workoutTemplateViewModel, DatabaseContext context, ExerciseDataManager exerciseData)
        {
            _workoutTemplateViewModel = workoutTemplateViewModel;
            _context = context;
            _exerciseData = exerciseData;
            exerciseGroupDict = ExerciseGroupDictionary.ExerciseGroupDict;
        }

        [ObservableProperty]
        private ObservableCollection<SetTemplate> setList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<Exercise> exerciseList = new();

        [ObservableProperty]
        private bool showStcList = false;

        [ObservableProperty]
        private bool showExerciseList = false;

        [ObservableProperty]
        private bool showAddSetMenu = false;

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task LoadExerciseListAsync()
        {
            await ExecuteAsync(async () =>
            {
                ExerciseList.Clear();

                var exercises = await _exerciseData.GetFullExerciseList();

                UpdateExerciseList(exercises);
            });
        }

        private void UpdateExerciseList(IEnumerable<Exercise> exercises)
        {
            if (exercises is not null && exercises.Any())
            {
                exercises ??= new ObservableCollection<Exercise>();

                foreach (var exercise in exercises)
                {
                    ExerciseList.Add(exercise);
                }
            }
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
        private async Task CreateNewSetListAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplateCollection newSet = new();
                await _context.AddItemAsync<SetTemplateCollection>(newSet);
                WorkoutTemplate.SetTemplateCollectionId = newSet.Id;
                await _context.UpdateItemAsync<WorkoutTemplate>(WorkoutTemplate);
            });

            OnPropertyChanged(nameof(WorkoutTemplate));

            await LoadSetListFromSetTemplateCollectionIdAsync();
        }

        [RelayCommand]
        public async Task LoadSetListFromSetTemplateCollectionIdAsync()
        {
            if (WorkoutTemplate is null) return;

            // Skip function if no SetTemplateCollectionId is set
            if (WorkoutTemplate.SetTemplateCollectionId == 0) return;

            SetList.Clear();

            Expression<Func<SetTemplate, bool>> predicate = entity => entity.SetTemplateCollectionId == WorkoutTemplate.SetTemplateCollectionId;

            try
            {
                var filteredList = await _context.GetFilteredAsync<SetTemplate>(predicate);

                foreach (var item in filteredList)
                {
                    SetList.Add(item);
                }

                if (!filteredList.Any())
                {
                    // If filteredList is empty
                    // Check if a SetTemplateCollection with that Id exists
                    await CheckIfSetTemplateCollectionExists();
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                return;
            }
        }

        private async Task CheckIfSetTemplateCollectionExists()
        {
            if (WorkoutTemplate is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.ItemExistsByKeyAsync<SetTemplateCollection>(WorkoutTemplate.SetTemplateCollectionId))
                {
                    // If no SetTemplateCollection with that Id exists
                    // Reset SetTemplateCollectionId value for current WorkoutTemplate
                    await ResetSetTemplateCollectionId();
                }
            });
        }

        [RelayCommand]
        private async Task ResetSetTemplateCollectionId()
        {
            if (WorkoutTemplate is null) return;

            WorkoutTemplate.SetTemplateCollectionId = 0;

            await UpdateWorkoutTemplateAsync();

            SetList.Clear();
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

        [RelayCommand]
        private async Task UpdateWorkoutTemplateAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WorkoutTemplate>(WorkoutTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout Template.", "OK");
                }

                OnPropertyChanged(nameof(WorkoutTemplate));
            });
        }

        [RelayCommand]
        private async Task ShowExistingWorkoutList()
        {
            await LoadWorkoutTemplatesAsync();
            ShowStcList = true;
        }

        [RelayCommand]
        private async Task CopyWorkout(int stcId)
        {
            if (WorkoutTemplate is null) return;

            if (stcId == WorkoutTemplate.SetTemplateCollectionId)
            {
                await Shell.Current.DisplayAlert("Error", "Can not copy the same workout!", "OK");
                return;
            }

            Expression<Func<SetTemplate, bool>> predicate = entity => entity.SetTemplateCollectionId == stcId;

            // Get all SetTemplates of old STC id
            IEnumerable<SetTemplate> filteredWtcList = null;
            try
            {
                filteredWtcList = await _context.GetFilteredAsync<SetTemplate>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Templates.", "OK");
            }

            if (!filteredWtcList.Any()) 
            {
                await Shell.Current.DisplayAlert("Error", "Set List is empty!", "OK");
                return;
            }

            // Create New SetTemplateCollection
            await CreateNewSetListAsync();

            // Get Id of new SetTemplateCollection
            int newStcId = WorkoutTemplate.SetTemplateCollectionId;

            SetList.Clear();

            // Copy old SetTemplates but with new SetTemplateCollectionId
            foreach (var item in filteredWtcList)
            {
                await ExecuteAsync(async () =>
                {
                    var itemCopy = item.Clone();
                    itemCopy.SetTemplateCollectionId = newStcId;
                    await _context.AddItemAsync<SetTemplate>(itemCopy);
                    SetList.Add(itemCopy);
                });
            }

            ShowStcList = false;
        }

        [RelayCommand]
        private void DisplayAddSetMenu()
        {
            ShowAddSetMenu = true;
        }

        [RelayCommand]
        private void DisplayExerciseList()
        {
            ShowExerciseList = true;
        }

        [RelayCommand]
        private async Task AddSetToSetList()
        {
            if (WorkoutTemplate is null) return;

            // Create new Set Template Collection if Workout Template does not have one assigned
            if (WorkoutTemplate.SetTemplateCollectionId == 0)
            {
                await CreateNewSetListAsync();
            }

        }
    }
}