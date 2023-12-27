using Microsoft.Maui;
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
        public int WorkoutTemplateId { get; set; }
        public string ExerciseName { get; set; }
        public string Note { get; set; }
        public bool IsWarmupSet { get; set; }
        public bool IsTrackingWeight { get; set; }
        public bool IsTrackingReps { get; set; }
        public bool IsTrackingRir {  get; set; }
        public bool IsTrackingRpe { get; set; }
        public bool IsTrackingTime { get; set; }
        public bool IsTrackingDistance { get; set; }
        public bool IsTrackingCardioResistance { get; set; }
        public bool IsUsingBodyWeightAsWeight { get; set; }

        public SetTemplate Clone() => MemberwiseClone() as SetTemplate;

        public Dictionary<bool, List<string>> GetTrackingDict()
        {
            List<string> isTrackingList = new();
            List<string> isNotTrackingList = new();

            (this.IsWarmupSet ? isTrackingList : isNotTrackingList).Add("Warmup");
            (this.IsTrackingWeight ? isTrackingList : isNotTrackingList).Add("Weight");
            (this.IsTrackingReps ? isTrackingList : isNotTrackingList).Add("Reps");
            (this.IsTrackingRir ? isTrackingList : isNotTrackingList).Add("RIR");
            (this.IsTrackingRpe ? isTrackingList : isNotTrackingList).Add("RPE");
            (this.IsTrackingTime ? isTrackingList : isNotTrackingList).Add("Time");
            (this.IsTrackingDistance ? isTrackingList : isNotTrackingList).Add("Distance");
            (this.IsTrackingCardioResistance ? isTrackingList : isNotTrackingList).Add("Cardio Resistance");
            (this.IsUsingBodyWeightAsWeight ? isTrackingList : isNotTrackingList).Add("Count Body Weight");

            Dictionary<bool, List<string>> setTrackingDict = new()
            {
                { true, isTrackingList },
                { false, isNotTrackingList }
            };

            return setTrackingDict;
        }
    }
}
