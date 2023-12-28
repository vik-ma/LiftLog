using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class UserPreferences
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsUsingMetricUnits { get; set; }
        public int ActiveWorkoutRoutineId { get; set; }
        public int ActiveTimePeriodId { get; set; }
        public bool ShowCompletedSetTimestamp { get; set; }
        public int DefaultBarbellWeight { get; set; }
        public int DefaultDumbellWeight { get; set; }
    }
}
