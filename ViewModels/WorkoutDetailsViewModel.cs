using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections;
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

        private List<int> SetListIdOrder = new();

        public WorkoutDetailsViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<SetTemplate> setList = new();

        [ObservableProperty]
        private ObservableCollection<WorkoutTemplate> workoutTemplateList = new();

        [ObservableProperty]
        private bool showWorkoutTemplateList = false;

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

        private static List<int> LoadSetListIdOrder(string setListOrderString)
        {
            if (string.IsNullOrEmpty(setListOrderString)) return new();

            string[] setList = setListOrderString.Split(',');

            List<int> setListOrderInt = new();

            foreach (string s in setList)
            {
                if (int.TryParse(s, out int setId))
                {
                    setListOrderInt.Add(setId);
                }
            }

            return setListOrderInt;
        }

        public async Task LoadSetListFromWorkoutTemplateIdAsync()
        {
            if (WorkoutTemplate is null) return;

            // Load SetListOrder as List<int>
            SetListIdOrder = LoadSetListIdOrder(WorkoutTemplate.SetListOrder);

            SetList.Clear();

            List<SetTemplate> setTemplateList = new();

            Expression<Func<SetTemplate, bool>> predicate = entity => entity.WorkoutTemplateId == WorkoutTemplate.Id;

            try
            {
                var filteredList = await _context.GetFilteredAsync<SetTemplate>(predicate);

                foreach (var item in filteredList)
                {
                    setTemplateList.Add(item);
                }

                if (setTemplateList.Any())
                {
                    SetList = new ObservableCollection<SetTemplate>(setTemplateList.OrderBy(obj => SetListIdOrder.IndexOf(obj.Id)));
                }

                OnPropertyChanged(nameof(WorkoutTemplate));
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                return;
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
            ShowWorkoutTemplateList = true;
        }

        [RelayCommand]
        private async Task CopyWorkout(int existingWorkoutId)
        {
            if (WorkoutTemplate is null) return;

            WorkoutTemplate existingWorkoutTemplate = WorkoutTemplateList.Where(p => p.Id == existingWorkoutId).FirstOrDefault();

            if (existingWorkoutTemplate is null) return;

            if (SetList.Any())
            {
                await Shell.Current.DisplayAlert("Error", "Can only copy workout if Set List is empty!", "OK");
                return;
            }

            if (existingWorkoutId == WorkoutTemplate.Id)
            {
                await Shell.Current.DisplayAlert("Error", "Can not copy the same workout!", "OK");
                return;
            }

            Expression<Func<SetTemplate, bool>> predicate = entity => entity.WorkoutTemplateId == existingWorkoutId;

            IEnumerable<SetTemplate> filteredList = null;
            try
            {
                // Get all SetTemplates of old WorkoutTemplate Id
                filteredList = await _context.GetFilteredAsync<SetTemplate>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Templates.", "OK");
            }

            if (!filteredList.Any())
            {
                await Shell.Current.DisplayAlert("Error", "Workout Set List is empty!", "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                // GET SETLISTORDER OF OLD WORKOUT
                //var setListOrder = existingWorkoutTemplate.SetListOrder;

                //var orderedFilteredList = filteredList.OrderBy(obj => setListOrder.IndexOf(obj.Id));


                // Copy old SetTemplates but with new WorkoutTemplateId
                //foreach (var item in orderedFilteredList)
                //{
                //    var itemCopy = item.Clone();
                //    itemCopy.WorkoutTemplateId = WorkoutTemplate.Id;
                //    await _context.AddItemAsync<SetTemplate>(itemCopy);
                //    SetList.Add(itemCopy);
                //}
            });

            await GenerateSetListOrderString();

            ShowWorkoutTemplateList = false;
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

            await LoadSetListFromWorkoutTemplateIdAsync();

            await GenerateSetListOrderString();
        }

        [RelayCommand]
        private async Task CreateNewSetTemplate()
        {
            if (WorkoutTemplate is null) return;

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