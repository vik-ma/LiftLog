using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public partial class SetTemplate : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int WorkoutTemplateId { get; set; }
        public string ExerciseName { get; set; }
        public string Note { get; set; }
        public bool IsWarmupSet { get; set; }
        [ObservableProperty]
        public bool isTrackingWeight;
        [ObservableProperty]
        public bool isTrackingReps;
        [ObservableProperty]
        public bool isTrackingRir;
        [ObservableProperty]
        public bool isTrackingRpe;
        [ObservableProperty]
        public bool isTrackingTime;
        [ObservableProperty]
        public bool isTrackingDistance;
        [ObservableProperty]
        public bool isTrackingCardioResistance;
        public bool IsUsingBodyWeightAsWeight { get; set; }
        [ObservableProperty]
        public int defaultWeightValue;
        [ObservableProperty]
        public int defaultRepsValue;
        [ObservableProperty]
        public int defaultRirValue;
        [ObservableProperty]
        public int defaultRpeValue;
        [ObservableProperty]
        public int defaultTimeValue;
        [ObservableProperty]
        public int defaultDistanceValue;
        [ObservableProperty]
        public int defaultCardioResistanceValue;

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
