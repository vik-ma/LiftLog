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

        public void ResetDefaultValuesIfNotTracking()
        {
            // Reset Default Values to 0 if tracking is turned off
            if (!IsTrackingWeight && DefaultWeightValue != 0) DefaultWeightValue = 0;
            if (!IsTrackingReps && DefaultRepsValue != 0) DefaultRepsValue = 0;
            if (!IsTrackingRir && DefaultRirValue != 0) DefaultRirValue = 0;
            if (!IsTrackingRpe && DefaultRpeValue != 0) DefaultRpeValue = 0;
            if (!IsTrackingTime && DefaultTimeValue != 0) DefaultTimeValue = 0;
            if (!IsTrackingDistance && DefaultDistanceValue != 0) DefaultDistanceValue = 0;
            if (!IsTrackingCardioResistance && DefaultCardioResistanceValue != 0) DefaultCardioResistanceValue = 0;
        }

        public bool ValidateAtLeastOnePropertyTracked()
        {
            // Returns true if any Set Property is tracked, otherwise false
            if (IsTrackingWeight) return true;
            if (IsTrackingReps) return true;
            if (IsTrackingRir) return true;
            if (IsTrackingRpe) return true;
            if (IsTrackingTime) return true;
            if (IsTrackingDistance) return true;
            if (IsTrackingCardioResistance) return true;

            return false;
        }
    }
}
