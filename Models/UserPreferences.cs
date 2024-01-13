namespace LocalLiftLog.Models
{
    public class UserPreferences
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsUsingMetricUnits { get; set; } = true;
        public bool IsUsing24HourClock { get; set; } = true;
        public int ActiveWorkoutRoutineId { get; set; }
        public int ActiveTimePeriodId { get; set; }
        public bool ShowCompletedSetTimestamp { get; set; } = true;
        public int DefaultBarbellWeight { get; set; }
        public int DefaultDumbellWeight { get; set; }
        public int ActiveUserWeightId { get; set; }

        public void ResetUserPreferences()
        {
            IsUsingMetricUnits = true;
            IsUsing24HourClock = true;
            ActiveWorkoutRoutineId = 0;
            ActiveTimePeriodId = 0;
            ShowCompletedSetTimestamp = true;
            DefaultBarbellWeight = 0;
            DefaultDumbellWeight = 0;
            ActiveUserWeightId = 0; 
        }
    }
}
