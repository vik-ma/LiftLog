﻿using CommunityToolkit.Mvvm.ComponentModel;
using LocalLiftLog.Helpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public partial class CompletedSet : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ExerciseName { get; set; }
        public string Note { get; set; }
        [ObservableProperty]
        public bool isCompleted;
        public int PercentCompleted { get; set; }
        public bool IsWarmupSet { get; set; }
        [ObservableProperty]
        public string timeCompleted;
        public int SetTemplateId { get; set; }
        public int CompletedWorkoutId { get; set; }
        public int Weight { get; set; }
        public int Reps { get; set; }
        public int Rir { get; set; }
        public int Rpe { get; set; }
        public int Time { get; set; }
        public int Distance { get; set; }
        public int CardioResistance { get; set; }
        public bool IsUsingBodyWeightAsWeight { get; set; }

        #nullable enable
        public (bool IsValid, string? ErrorMessage) ValidatePercentCompletedValue()
        {
            if (PercentCompleted < 0 || PercentCompleted > 100)
            {
                return (false, "Invalid Percent Value.");
            }
            return (true, null);
        }

        public (bool IsValid, string? ErrorMessage) ValidateTrackingValues()
        {
            int minValue = ConstantsHelper.CompletedSetMinValue;
            int maxValue = ConstantsHelper.CompletedSetMaxValue;

            if (Weight < minValue || Weight > maxValue) return (false, "Weight");
            if (Reps < minValue || Reps > maxValue) return (false, "Reps");
            if (Rir < minValue || Rir > maxValue) return (false, "RIR");
            if (Rpe < minValue || Rpe > maxValue) return (false, "RPE");
            if (Time < minValue || Time > maxValue) return (false, "Time");
            if (Distance < minValue || Distance > maxValue) return (false, "Distance");
            if (CardioResistance < minValue || CardioResistance > maxValue) return (false, "Cardio Resistance");

            return (true, null);
        }
    }
}
