﻿namespace LocalLiftLog.Models
{
    public class UserPreferences
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsUsing24HourClock { get; set; } = true;
        public int ActiveWorkoutRoutineId { get; set; }
        public int ActiveTimePeriodId { get; set; }
        public bool ShowCompletedSetTimestamp { get; set; } = true;
        public int ActiveUserWeightId { get; set; }
        public string DefaultWeightUnit { get; set; } = "kg";
        public string DefaultDistanceUnit { get; set; } = "km";

        public void ResetUserPreferences()
        {
            IsUsing24HourClock = true;
            ActiveWorkoutRoutineId = 0;
            ActiveTimePeriodId = 0;
            ShowCompletedSetTimestamp = true;
            ActiveUserWeightId = 0;
            DefaultWeightUnit = "kg";
            DefaultDistanceUnit = "km";
        }
    }
}
