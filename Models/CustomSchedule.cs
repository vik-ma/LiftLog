using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class CustomSchedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ScheduleFactoryId { get; set; }
        public int WorkoutRoutineId { get; set; }
        public int NumDaysInSchedule { get; set; }
        public string ScheduleStartDate { get; set; }
    }
}
