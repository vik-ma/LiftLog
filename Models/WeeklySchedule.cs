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
        public int Day1WorkoutTemplateListId { get; set; }
        public int Day2WorkoutTemplateListId { get; set; }
        public int Day3WorkoutTemplateListId { get; set; }
        public int Day4WorkoutTemplateListId { get; set; }
        public int Day5WorkoutTemplateListId { get; set; }
        public int Day6WorkoutTemplateListId { get; set; }
        public int Day7WorkoutTemplateListId { get; set; }

        public WeeklySchedule Clone() => MemberwiseClone() as WeeklySchedule;

    }
}
