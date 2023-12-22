using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class WorkoutTemplateCollection
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Day { get; set; }
        public int WorkoutTemplateId { get; set; }
        public string WorkoutTemplateName { get; set; }
        public int ScheduleFactoryId { get; set; }
        public int WorkoutRoutineId { get; set; }
    }

}