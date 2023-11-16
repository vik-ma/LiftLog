using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class SetTemplate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string Note { get; set; }
        public bool IsTrackingWeight { get; set; }
        public bool IsTrackingReps { get; set; }
        public bool IsTrackingRir {  get; set; }
        public bool IsTrackingRpe { get; set; }
        public bool IsTrackingTime { get; set; }
        public bool IsTrackingDistance { get; set; }
        public bool IsTrackingCardioResistance { get; set; }
        public bool IsUsingBodyWeightAsWeight { get; set; }
    }
}
