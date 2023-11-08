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
        public int NumDaysInSchedule { get; set; }
        public string ScheduleStartDate { get; set; }
        public int Day1WorkoutTemplateCollectionId { get; set; }
        public int Day2WorkoutTemplateCollectionId { get; set; }
        public int Day3WorkoutTemplateCollectionId { get; set; }
        public int Day4WorkoutTemplateCollectionId { get; set; }
        public int Day5WorkoutTemplateCollectionId { get; set; }
        public int Day6WorkoutTemplateCollectionId { get; set; }
        public int Day7WorkoutTemplateCollectionId { get; set; }
        public int Day8WorkoutTemplateCollectionId { get; set; }
        public int Day9WorkoutTemplateCollectionId { get; set; }
        public int Day10WorkoutTemplateCollectionId { get; set; }
        public int Day11WorkoutTemplateCollectionId { get; set; }
        public int Day12WorkoutTemplateCollectionId { get; set; }
        public int Day13WorkoutTemplateCollectionId { get; set; }
        public int Day14WorkoutTemplateCollectionId { get; set; }

    }
}
