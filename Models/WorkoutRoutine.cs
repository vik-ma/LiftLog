﻿namespace LocalLiftLog.Models
{
    public class WorkoutRoutine
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastUpdateDateTime { get; set; }  
        public string Note { get; set; }
        public bool IsScheduleWeekly { get; set; } = true;
        public int NumDaysInSchedule { get; set; } = 7;
        public string CustomScheduleStartDate { get; set; }

        public WorkoutRoutine Clone() => MemberwiseClone() as WorkoutRoutine;

        #nullable enable
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, $"{nameof(Name)} is required.");
            }
            return (true, null);
        }
        public void UpdateDateTime()
        {
            LastUpdateDateTime = DateTimeHelper.GetCurrentFormattedDateTime();
        }

        public void ResetSchedule()
        {
            IsScheduleWeekly = true;
            NumDaysInSchedule = 7;
            CustomScheduleStartDate = null;
        }

        public void CopySchedulePropertiesFromOtherWorkoutRoutine(WorkoutRoutine otherWorkoutRoutine)
        {
            if (otherWorkoutRoutine is null) return;

            IsScheduleWeekly = otherWorkoutRoutine.IsScheduleWeekly;
            NumDaysInSchedule = otherWorkoutRoutine.NumDaysInSchedule;
            CustomScheduleStartDate = otherWorkoutRoutine.CustomScheduleStartDate;
        }
    }
}
