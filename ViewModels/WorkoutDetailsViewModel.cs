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

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        public WorkoutDetailsViewModel(WorkoutTemplateViewModel workoutTemplateViewModel, DatabaseContext context)
        {
            _workoutTemplateViewModel = workoutTemplateViewModel;
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<SetTemplate> setList = new();

        [ObservableProperty]
        private ObservableCollection<SetTemplateCollection> setTemplateCollectionList = new();

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
                WorkoutTemplate.SetTemplateCollectionId = newSet.Id;
                await _context.UpdateItemAsync<WorkoutTemplate>(WorkoutTemplate);
            });

            OnPropertyChanged(nameof(WorkoutTemplate));
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

        public async Task LoadSetTemplateCollectionsAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplateCollectionList.Clear();

                var setTemplatesCollections = await _context.GetAllAsync<SetTemplateCollection>();

                if (setTemplatesCollections is not null && setTemplatesCollections.Any())
                {
                    setTemplatesCollections ??= new ObservableCollection<SetTemplateCollection>();

                    foreach (var setCollection in setTemplatesCollections)
                    {
                        SetTemplateCollectionList.Add(setCollection);
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
        private void ShowExistingSetTemplateCollectionList()
        {
            ShowStcList = true;
        }

        [RelayCommand]
        private async Task CopySetTemplateCollection(int stcId)
        {
            if (WorkoutTemplate is null) return;

            // Create New SetTemplateCollection
            await CreateNewSetListAsync();

            // Get Id of new SetTemplateCollection
            int newStcId = WorkoutTemplate.SetTemplateCollectionId;

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

            await LoadSetTemplateCollectionsAsync();

            ShowStcList = false;
        }
    }
}