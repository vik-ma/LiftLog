using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalLiftLog.Models
{
    public class WeeklySchedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int WorkoutRoutineId { get; set; }
    }
}
