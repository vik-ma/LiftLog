using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Data
{
    public static class ExerciseGroupDictionary
    {
        public static Dictionary<int, string> ExerciseGroupDict { get; } = new()
        {
            { 0, "Chest" },
            { 1, "Triceps" },
            { 2, "Biceps" },
            { 3, "Shoulders" },
            { 4, "Upper Back" },
            { 5, "Mid Back (Lats)" },
            { 6, "Lower Back" },
            { 7, "Quadriceps" },
            { 8, "Glutes" },
            { 9, "Hamstrings" },
            { 10, "Calves" },
            { 11, "Forearms" },
            { 12, "Core (Abs)" },
            { 13, "Grip" },
            { 14, "Cardio" },
            { 15, "Other" },
        };
    }
}
