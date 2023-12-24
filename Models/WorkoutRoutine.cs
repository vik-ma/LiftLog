using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Helpers;

namespace LocalLiftLog.Models
{
    public class WorkoutRoutine
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastUpdateDateTime { get; set; }  
        public string Note { get; set; }
        public bool IsScheduleWeekly { get; set; }
        public int ScheduleId { get; set; }

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
    }
}
