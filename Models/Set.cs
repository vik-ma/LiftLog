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
        public int SetTemplateId { get; set; }
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
        public bool isUsingBodyWeightAsWeight;
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

        #nullable enable
        public (bool IsValid, string? ErrorMessage) ValidateTrackingValues()
        {
            int minValue = ConstantsHelper.CompletedSetMinValue;
            int maxValue = ConstantsHelper.CompletedSetMaxValue;

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

        public (bool IsValid, string? ErrorMessage) ValidateSet()
        {
            if (ExerciseId < 1) return (false, "Invalid Exercise Id");

            if (PercentCompleted < 0 || PercentCompleted > 100) return (false, "Invalid Percent Value");

            var (isTrackingValueValid, TrackingValueErrorMessage) = ValidateTrackingValues();
            if (!isTrackingValueValid) return (false, $"Invalid {TrackingValueErrorMessage} Value");

            var (isUnitValueValid, UnitValueErrorMessage) = ValidateUnits();
            if (!isUnitValueValid) return (false, $"Invalid {UnitValueErrorMessage} Unit Value");

            return (true, null);
        }

    }
}
