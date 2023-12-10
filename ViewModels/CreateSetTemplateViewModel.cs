using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutTemplate), nameof(WorkoutTemplate))]
    public partial class CreateSetTemplateViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        private readonly ExerciseDataManager _exerciseData;

        private readonly Dictionary<int, string> exerciseGroupDict;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        public CreateSetTemplateViewModel(DatabaseContext context, ExerciseDataManager exerciseData)
        {
            _context = context;
            _exerciseData = exerciseData;
            exerciseGroupDict = ExerciseGroupDictionary.ExerciseGroupDict;
        }

        [ObservableProperty]
        private ObservableCollection<Exercise> exerciseList = new();

        [ObservableProperty]
        private SetTemplate operatingSetTemplate = new();

        [ObservableProperty]
        private int newSetTemplateNumSets;

        [ObservableProperty]
        private string newSetTemplateSelectedExerciseName;

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task LoadExerciseListAsync()
        {
            await ExecuteAsync(async () =>
            {
                var exercises = await _exerciseData.GetFullExerciseList();

                ExerciseList.Clear();

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
        private async Task CreateNewSetTemplate()
        {
            if (OperatingSetTemplate is null) return;

            // Create new Set Template Collection if Workout Template does not have one assigned
            //if (activeSetCollectionId == 0)
            //{
            //    await CreateNewSetListAsync();
            //}

            SetTemplate newSetTemplate = new()
            {
                //SetTemplateCollectionId = activeSetCollectionId,
                ExerciseName = NewSetTemplateSelectedExerciseName,
                Note = OperatingSetTemplate.Note,
                IsTrackingWeight = OperatingSetTemplate.IsTrackingWeight,
                IsTrackingReps = OperatingSetTemplate.IsTrackingReps,
                IsTrackingRir = OperatingSetTemplate.IsTrackingRir,
                IsTrackingRpe = OperatingSetTemplate.IsTrackingRpe,
                IsTrackingTime = OperatingSetTemplate.IsTrackingTime,
                IsTrackingDistance = OperatingSetTemplate.IsTrackingDistance,
                IsTrackingCardioResistance = OperatingSetTemplate.IsTrackingCardioResistance,
                IsUsingBodyWeightAsWeight = OperatingSetTemplate.IsUsingBodyWeightAsWeight
            };

            int numSets = NewSetTemplateNumSets;

            // Add validation

            await SaveSetTemplateAsync(newSetTemplate, numSets);


        }

        private async Task SaveSetTemplateAsync(SetTemplate setTemplate, int numSets)
        {
            // Check if numSets > 0 and < 20

            if (setTemplate == null) return;

            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<SetTemplate>(setTemplate);
            });
        }
    }
}
