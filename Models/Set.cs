namespace LocalLiftLog.Models
{
    public partial class Set : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public bool IsTemplate { get; set; }
        public int WorkoutTemplateId { get; set; }
        public string Note { get; set; }
        [ObservableProperty]
        public string comment;
        [ObservableProperty]
        public bool isWarmupSet;
        [ObservableProperty]
        public bool isTrackingWeight;
        [ObservableProperty]
        public bool isTrackingReps;
        [ObservableProperty]
        public bool isTrackingRir;
        [ObservableProperty]
        public bool isTrackingRpe;
        [ObservableProperty]
        public bool isTrackingTime;
        [ObservableProperty]
        public bool isTrackingDistance;
        [ObservableProperty]
        public bool isTrackingCardioResistance;
        [ObservableProperty]
        public bool isTrackingPercentCompleted;
        [ObservableProperty]
        public bool isCompleted;
        [ObservableProperty]
        public string timeCompleted;
        [ObservableProperty]
        public double weight;
        [ObservableProperty]
        public int reps;
        [ObservableProperty]
        public int rir;
        [ObservableProperty]
        public int rpe;
        [ObservableProperty]
        public int timeInSeconds;
        [ObservableProperty]
        public double distance;
        [ObservableProperty]
        public double cardioResistance;
        [ObservableProperty]
        public string weightUnit;
        [ObservableProperty]
        public string distanceUnit;
        [ObservableProperty]
        public int percentCompleted;
        public double UserWeight { get; set; }

        public Set Clone() => MemberwiseClone() as Set;

        #nullable enable
        public (bool IsValid, string? ErrorMessage) ValidateTrackingValues()
        {
            int minValue = ConstantsHelper.SetTrackingMinValue;
            int maxValue = ConstantsHelper.SetTrackingMaxValue;

            if (Weight < minValue || Weight > maxValue) return (false, "Weight");
            if (Reps < minValue || Reps > maxValue) return (false, "Reps");
            if (Rir < minValue || Rir > maxValue) return (false, "RIR");
            if (Rpe < minValue || Rpe > maxValue) return (false, "RPE");
            if (TimeInSeconds < minValue || TimeInSeconds > maxValue) return (false, "Time");
            if (Distance < minValue || Distance > maxValue) return (false, "Distance");
            if (CardioResistance < minValue || CardioResistance > maxValue) return (false, "Cardio Resistance");

            return (true, null);
        }

        public (bool IsValid, string? ErrorMessage) ValidateUnits()
        {
            if (!ConstantsHelper.ValidWeightUnits.Contains(WeightUnit)) return (false, "Weight");
            if (!ConstantsHelper.ValidDistanceUnits.Contains(DistanceUnit)) return (false, "Distance");

            return (true, null);
        }

        public bool ValidateAtLeastOnePropertyTracked()
        {
            // Returns true if any Set Property is tracked, otherwise false
            if (IsTrackingWeight) return true;
            if (IsTrackingReps) return true;
            if (IsTrackingRir) return true;
            if (IsTrackingRpe) return true;
            if (IsTrackingTime) return true;
            if (IsTrackingDistance) return true;
            if (IsTrackingCardioResistance) return true;
            if (IsTrackingPercentCompleted) return true;

            return false;
        }

        public bool ValidateTemplateId()
        {
            if (IsTemplate && WorkoutTemplateId == 0) return false;

            return true;
        }

        public bool ValidateRpe()
        {
            if (Rpe < 0 || Rpe > 10) return false;

            if (Rpe == 0 && IsTrackingRpe) return false;

            return true;
        }

        public (bool IsValid, string? ErrorMessage) ValidateSet()
        {
            if (ExerciseId < 1) return (false, $"Invalid Exercise Id {ExerciseId}");

            if (PercentCompleted < 0 || PercentCompleted > 100) return (false, "Invalid Percent Value");

            // Validate that at least one Set Property tracked
            if (!ValidateAtLeastOnePropertyTracked()) return (false, "At least one property must be tracked!");

            // Validate that all Default Values are within min and max range
            var (isTrackingValueValid, TrackingValueErrorMessage) = ValidateTrackingValues();
            if (!isTrackingValueValid) return (false, $"Invalid {TrackingValueErrorMessage} Value");

            var (isUnitValueValid, UnitValueErrorMessage) = ValidateUnits();
            if (!isUnitValueValid) return (false, $"Invalid {UnitValueErrorMessage} Unit Value");

            // Validate that Set has a WorkoutTemplateId if it is Template
            if (!ValidateTemplateId()) return (false, "Invalid Workout Template");

            // Validate that RPE is between 0 or 10, and not 0 if IsTrackingRpe is true
            if (!ValidateRpe()) return (false, "Invalid RPE Value");

            return (true, null);
        }

        public Dictionary<bool, List<string>> GetTrackingDict()
        {
            List<string> isTrackingList = new();
            List<string> isNotTrackingList = new();

            (IsWarmupSet ? isTrackingList : isNotTrackingList).Add("Warmup");
            (IsTrackingWeight ? isTrackingList : isNotTrackingList).Add("Weight");
            (IsTrackingReps ? isTrackingList : isNotTrackingList).Add("Reps");
            (IsTrackingRir ? isTrackingList : isNotTrackingList).Add("RIR");
            (IsTrackingRpe ? isTrackingList : isNotTrackingList).Add("RPE");
            (IsTrackingTime ? isTrackingList : isNotTrackingList).Add("Time");
            (IsTrackingDistance ? isTrackingList : isNotTrackingList).Add("Distance");
            (IsTrackingCardioResistance ? isTrackingList : isNotTrackingList).Add("Cardio Resistance");
            (IsTrackingPercentCompleted ? isTrackingList : isNotTrackingList).Add("Percent Completed");

            Dictionary<bool, List<string>> setTrackingDict = new()
            {
                { true, isTrackingList },
                { false, isNotTrackingList }
            };

            return setTrackingDict;
        }

        public void ResetDefaultValuesIfNotTracking()
        {
            // Reset Default Values to 0 if tracking is turned off
            if (!IsTrackingWeight && Weight != 0) Weight = 0;
            if (!IsTrackingReps && Reps != 0) Reps = 0;
            if (!IsTrackingRir && Rir != 0) Rir = 0;
            if (!IsTrackingRpe && Rpe != 0) Rpe = 0;
            if (!IsTrackingTime && TimeInSeconds != 0) TimeInSeconds = 0;
            if (!IsTrackingDistance && Distance != 0) Distance = 0;
            if (!IsTrackingCardioResistance && CardioResistance != 0) CardioResistance = 0;
            if (!IsTrackingPercentCompleted && PercentCompleted != 0) PercentCompleted = 0;
        }
    }
}
