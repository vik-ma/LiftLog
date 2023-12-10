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
    [QueryProperty(nameof(SetWorkoutTemplatePackage), nameof(SetWorkoutTemplatePackage))]
    public partial class CreateSetTemplateViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        private readonly ExerciseDataManager _exerciseData;

        private readonly Dictionary<int, string> exerciseGroupDict;

        [ObservableProperty]
        private SetWorkoutTemplatePackage setWorkoutTemplatePackage;

        [ObservableProperty]
        private SetTemplate operatingSetTemplate;

        [ObservableProperty]
        private WorkoutTemplate operatingWorkoutTemplate;

        public CreateSetTemplateViewModel(DatabaseContext context, ExerciseDataManager exerciseData)
        {
            _context = context;
            _exerciseData = exerciseData;
            exerciseGroupDict = ExerciseGroupDictionary.ExerciseGroupDict;
        }

        [ObservableProperty]
        private ObservableCollection<Exercise> exerciseList = new();

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

        public void InitializeSetWorkoutTemplatePackage()
        {
            OperatingSetTemplate = SetWorkoutTemplatePackage.setTemplate;
            OperatingWorkoutTemplate = SetWorkoutTemplatePackage.workoutTemplate;
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
            if (OperatingSetTemplate is null || OperatingWorkoutTemplate is null) return;

            // CHECK IF ID EXISTS

            SetTemplate newSetTemplate = new()
            {
                SetTemplateCollectionId = OperatingWorkoutTemplate.SetTemplateCollectionId,
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
            if (numSets < 1 || numSets > 10)
            {
                await Shell.Current.DisplayAlert("Error", "Number of sets must be between 1 and 10!", "OK");
                return;
            }

            if (setTemplate == null) return;

            await ExecuteAsync(async () =>
            {
                for (int i = 0; i < numSets; i++) {
                    await _context.AddItemAsync<SetTemplate>(setTemplate);
                }
            });

            await GoBack();
        }
    }
}
