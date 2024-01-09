using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using LocalLiftLog.Helpers;
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

        [ObservableProperty]
        private bool isEditing;

        public CreateSetTemplateViewModel(DatabaseContext context, ExerciseDataManager exerciseData)
        {
            _context = context;
            _exerciseData = exerciseData;
            exerciseGroupDict = ExerciseGroupDictionary.ExerciseGroupDict;
        }

        [ObservableProperty]
        public List<Exercise> exerciseList = new();

        [ObservableProperty]
        public ObservableCollection<Exercise> filteredExerciseList = new();

        [ObservableProperty]
        private int newSetTemplateNumSets = 1;

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
            OperatingSetTemplate = SetWorkoutTemplatePackage.SetTemplate;
            OperatingWorkoutTemplate = SetWorkoutTemplatePackage.WorkoutTemplate;
            IsEditing = SetWorkoutTemplatePackage.IsEditing;

            if (IsEditing) NewSetTemplateSelectedExerciseName = OperatingSetTemplate.ExerciseName;
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

                FilteredExerciseList = new(ExerciseList);
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
        private async Task SaveSetTemplateAsync()
        {
            if (OperatingSetTemplate is null || OperatingWorkoutTemplate is null) return;

            if (IsEditing)
            {
                // If editing existing SetTemplate
                await UpdateSetTemplateAsync();
            }
            else
            {
                // If creating new SetTemplate(s)
                SetTemplate newSetTemplate = new()
                {
                    WorkoutTemplateId = OperatingWorkoutTemplate.Id,
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

                // Validate SetTemplate properties
                var validateResult = newSetTemplate.ValidateSetTemplate();
                if (!validateResult.IsValid)
                {
                    await Shell.Current.DisplayAlert("Error", validateResult.ErrorMessage, "OK");
                    return;
                }

                await CreateNewSetTemplateAsync(newSetTemplate, numSets);
            }
        }

        [RelayCommand]
        private async Task UpdateSetTemplateAsync()
        {
            if (OperatingSetTemplate is null) return;

            OperatingSetTemplate.ExerciseName = NewSetTemplateSelectedExerciseName;

            // Validate SetTemplate properties
            var validateResult = OperatingSetTemplate.ValidateSetTemplate();
            if (!validateResult.IsValid)
            {
                await Shell.Current.DisplayAlert("Error", validateResult.ErrorMessage, "OK");
                return;
            }

            if (!await _context.UpdateItemAsync<SetTemplate>(OperatingSetTemplate))
            {
                await Shell.Current.DisplayAlert("Error", "Error occured when updating Set Template.", "OK");
            }
            else
            {
                await GoBack();
            }
        }

        private async Task UpdateWorkoutTemplateAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<WorkoutTemplate>(OperatingWorkoutTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout Template.", "OK");
                }
            });
        }

        private async Task CreateNewSetTemplateAsync(SetTemplate setTemplate, int numSets)
        {
            if (numSets < 1 || numSets > 10)
            {
                await Shell.Current.DisplayAlert("Error", "Number of sets must be between 1 and 10!", "OK");
                return;
            }

            if (setTemplate == null) return;

            if (OperatingWorkoutTemplate is null) return;

            List<string> newSetList;

            if (!string.IsNullOrEmpty(OperatingWorkoutTemplate.SetListOrder))
            {
                // Create list of Ids from comma separated SetListOrder string if it exists
                newSetList = OperatingWorkoutTemplate.SetListOrder.Split(',').ToList();
            }
            else
            {
                // Create empty list if SetListOrder string is null or empty
                newSetList = new();
            }
            
            await ExecuteAsync(async () =>
            {
                for (int i = 0; i < numSets; i++) {
                    await _context.AddItemAsync<SetTemplate>(setTemplate);
                    newSetList.Add(setTemplate.Id.ToString());
                }
            });

            OperatingWorkoutTemplate.SetListOrder = string.Join(",", newSetList);

            await UpdateWorkoutTemplateAsync();

            await GoBack();
        }

        [RelayCommand]
        private async Task AddDefaultPropertyValue(string property)
        {
            string enteredNumber = await Shell.Current.DisplayPromptAsync("Default Value", "Enter Default Value\n", "OK", "Cancel");

            if (enteredNumber == null) return;

            bool validInput = int.TryParse(enteredNumber, out int enteredNumberInt);

            if (!validInput || enteredNumberInt < ConstantsHelper.SetTemplateDefaultInputMinValue || enteredNumberInt > ConstantsHelper.SetTemplateDefaultMaxValue)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Input Value.", "OK");
                return;
            }

            ChangeSetTemplatePropertyValue(property, enteredNumberInt);
        }

        [RelayCommand]
        private void RemoveDefaultPropertyValue(string property)
        {
            ChangeSetTemplatePropertyValue(property, 0);
        }

        private void ChangeSetTemplatePropertyValue(string property, int propertyValue)
        {
            switch (property)
            {
                case "Weight":
                    OperatingSetTemplate.DefaultWeightValue = propertyValue;
                    break;

                case "Reps":
                    OperatingSetTemplate.DefaultRepsValue = propertyValue;
                    break;

                case "Rir":
                    OperatingSetTemplate.DefaultRirValue = propertyValue;
                    break;

                case "Rpe":
                    OperatingSetTemplate.DefaultRpeValue = propertyValue;
                    break;

                case "Time":
                    OperatingSetTemplate.DefaultTimeValue = propertyValue;
                    break;

                case "Distance":
                    OperatingSetTemplate.DefaultDistanceValue = propertyValue;
                    break;

                case "CardioResistance":
                    OperatingSetTemplate.DefaultCardioResistanceValue = propertyValue;
                    break;

                default:
                    return;
            }
        }
    }
}
