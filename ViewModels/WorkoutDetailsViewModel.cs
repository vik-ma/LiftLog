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
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        private readonly List<int> SetListIdOrder = new();

        public WorkoutDetailsViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<SetTemplate> setList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

        [ObservableProperty]
        private bool showStcList = false;

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
        private async Task CreateNewSetListAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplateCollection newSet = new();
                await _context.AddItemAsync<SetTemplateCollection>(newSet);
                int newStcId = newSet.Id;
                WorkoutTemplate.SetTemplateCollectionId = newStcId;
                await _context.UpdateItemAsync<WorkoutTemplate>(WorkoutTemplate);
            });

            OnPropertyChanged(nameof(WorkoutTemplate));

            await LoadSetListFromSetTemplateCollectionIdAsync();
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
        }

        public async Task LoadSetListFromSetTemplateCollectionIdAsync()
        {
            if (WorkoutTemplate is null) return;

            // Skip function if no SetTemplateCollectionId is set
            if (WorkoutTemplate.SetTemplateCollectionId == 0) return;

            // Load SetListOrder as List<int>
            LoadSetListIdOrder();

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

            await GenerateSetListOrderString();
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
        private async Task GoToCreateSetTemplatePage(SetWorkoutTemplatePackage package)
        {
            if (package is null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                ["SetWorkoutTemplatePackage"] = package
            };

            await Shell.Current.GoToAsync($"{nameof(CreateSetTemplatePage)}?Id={WorkoutTemplate.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task DeleteSetTemplateAsync(int id)
        {
            SetTemplate setTemplate = SetList.FirstOrDefault(p => p.Id == id);

            if (setTemplate is null) return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<SetTemplate>(setTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set.", "OK");
                }
            });

            await LoadSetListFromSetTemplateCollectionIdAsync();
        }

        [RelayCommand]
        private async Task CreateNewSetTemplate()
        {
            if (WorkoutTemplate is null) return;

            // Create new Set Template Collection if Workout Template does not have one assigned
            if (WorkoutTemplate.SetTemplateCollectionId == 0)
            {
                await CreateNewSetListAsync();
            }

            SetWorkoutTemplatePackage package = new()
            {
                WorkoutTemplate = WorkoutTemplate,
                SetTemplate = new(),
                IsEditing = false
            };

            await GoToCreateSetTemplatePage(package);
        }

        [RelayCommand]
        private async Task EditSetTemplateAsync(int id)
        {
            if (WorkoutTemplate is null) return;

            SetTemplate selectedSetTemplate = SetList.FirstOrDefault(p => p.Id == id);

            if (selectedSetTemplate is null) return;

            SetWorkoutTemplatePackage package = new()
            {
                WorkoutTemplate = WorkoutTemplate,
                SetTemplate = selectedSetTemplate,
                IsEditing = true
            };

            await GoToCreateSetTemplatePage(package);
        }

        public async Task GenerateSetListOrderString()
        {
            if (WorkoutTemplate is null) return;

            IEnumerable<string> setIdList = SetList.Select(set => set.Id.ToString());

            WorkoutTemplate.SetListOrder = string.Join(",", setIdList);

            await UpdateWorkoutTemplateAsync();
        }

        [RelayCommand]
        private async Task MoveSetUp(int id)
        {
            SetTemplate setTemplate = SetList.Where(p => p.Id == id).FirstOrDefault();

            if (setTemplate is null) return;

            // Get SetList index of current Set
            int setIndex = SetList.IndexOf(setTemplate);

            // Do nothing if item is already first in list
            if (setIndex < 1) return;

            // Swap current Set with Set at the index before
            (SetList[setIndex - 1], SetList[setIndex]) = (SetList[setIndex], SetList[setIndex - 1]);
            
            await GenerateSetListOrderString();
        }

        [RelayCommand]
        private async Task MoveSetDown(int id)
        {
            SetTemplate setTemplate = SetList.Where(p => p.Id == id).FirstOrDefault();

            if (setTemplate is null) return;

            // Get SetList index of current Set
            int setIndex = SetList.IndexOf(setTemplate);

            // Do nothing if item is already last in list
            if (setIndex > SetList.Count - 2) return;

            // Swap current Set with Set at the index after
            (SetList[setIndex + 1], SetList[setIndex]) = (SetList[setIndex], SetList[setIndex + 1]);

            await GenerateSetListOrderString();
        }
    }
}