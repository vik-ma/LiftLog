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
        public int DefaultWeightValue { get; set; }
        public int DefaultWeightReps { get; set; }
        public int DefaultWeightRir { get; set; }
        public int DefaultWeightRpe { get; set; }
        public int DefaultWeightTime { get; set; }
        public int DefaultWeightDistance { get; set; }
        public int DefaultWeightCardioResistance { get; set; }

        public SetTemplate Clone() => MemberwiseClone() as SetTemplate;

        public Dictionary<bool, List<string>> GetTrackingDict()
        {
            List<string> isTrackingList = new();
            List<string> isNotTrackingList = new();

            (IsWarmupSet ? isTrackingList : isNotTrackingList).Add("Warmup");
            (IsTrackingWeight ? isTrackingList : isNotTrackingList).Add("Weight");
            (IsTrackingReps ? isTrackingList : isNotTrackingList).Add("Reps");
            (IsTrackingRir ? isTrackingList : isNotTrackingList).Add("RIR");
            (IsTrackingRpe ? isTrackingList : isNotTrackingList).Add("RPE");
            (IsTrackingTime ? isTrackingList : isNotTrackingList).Add("Time");
            (IsTrackingDistance ? isTrackingList : isNotTrackingList).Add("Distance");
            (IsTrackingCardioResistance ? isTrackingList : isNotTrackingList).Add("Cardio Resistance");
            (IsUsingBodyWeightAsWeight ? isTrackingList : isNotTrackingList).Add("Count Body Weight");

            Dictionary<bool, List<string>> setTrackingDict = new()
            {
                { true, isTrackingList },
                { false, isNotTrackingList }
            };

            return setTrackingDict;
        }
    }
}
