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

        public StartedWorkoutViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        private ObservableCollection<SetTemplate> setList = new();

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
        private async Task GoToCreateSetTemplatePage(int id)
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
    }
}
