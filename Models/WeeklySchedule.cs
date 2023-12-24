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
        public int Day1WorkoutTemplateCollectionId { get; set; }
        public int Day2WorkoutTemplateCollectionId { get; set; }
        public int Day3WorkoutTemplateCollectionId { get; set; }
        public int Day4WorkoutTemplateCollectionId { get; set; }
        public int Day5WorkoutTemplateCollectionId { get; set; }
        public int Day6WorkoutTemplateCollectionId { get; set; }
        public int Day7WorkoutTemplateCollectionId { get; set; }

    }
}
