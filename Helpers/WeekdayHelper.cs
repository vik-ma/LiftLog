using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Helpers
{
    public static class WeekdayHelper
    {
        public static Dictionary<int, string> WeekdayDict { get; } = new()
        {
            { 0, "Monday" },
            { 1, "Tuesday" },
            { 2, "Wednesday" },
            { 3, "Thursday" },
            { 4, "Friday" },
            { 5, "Saturday" },
            { 6, "Sunday" }
        };

        public static List<string> WeekdayList { get; } = new()
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
        };
    }
}
