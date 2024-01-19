namespace LocalLiftLog.Models
{
    public partial class Set : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        [ObservableProperty]
        public string note;
        public bool IsWarmupSet { get; set; }
        [ObservableProperty]
        public string timeCompleted;
        public double Weight { get; set; }
        public string WeightUnit { get; set; }
        public int Reps { get; set; }
        public int Rir { get; set; }
        public int Rpe { get; set; }
        public int TimeInSeconds { get; set; }
        public double Distance { get; set; }
        public string DistanceUnit { get; set; }
        public double CardioResistance { get; set; }
        public int PercentCompleted { get; set; }

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
            if (string.IsNullOrWhiteSpace(ExerciseName)) return (false, "Invalid Exercise Name");

            if (ExerciseId < 1) return (false, "Invalid Exercise Id");

            if (PercentCompleted < 0 || PercentCompleted > 100) return (false, "Invalid Percent Value");

            var (isTrackingValueValid, TrackingValueErrorMessage) = ValidateTrackingValues();
            if (!isTrackingValueValid) return (false, $"Invalid {TrackingValueErrorMessage} Value");

            var (isUnitValueValid, UnitValueErrorMessage) = ValidateTrackingValues();
            if (!isUnitValueValid) return (false, $"Invalid {UnitValueErrorMessage} Unit Value");

            return (true, null);
        }

    }
}
