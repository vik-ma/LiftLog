using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Data
{
    public static class SetProperties
    {
        public static readonly HashSet<string> SetPropertyHashSet = new()
        {
            "Warmup",
            "Weight",
            "Reps",
            "RIR",
            "RPE",
            "Time",
            "Distance",
            "Cardio Resistance",
            "Percent Completed",
            "Count Body Weight"
        };
    }
}
