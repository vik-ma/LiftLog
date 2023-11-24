using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Helpers
{
    public class DateTimeHelper
    {
        public static string GetCurrentFormattedDateTime()
        {
            string dateTimeString = DateTime.UtcNow.ToString();
            return dateTimeString;
        }

        public static bool ValidateStartAndEndDate(string startDateTime, string endDateTime)
        {
            // Parse the DateTime strings into DateTime objects
            if (DateTime.TryParse(startDateTime, out DateTime start) &&
                DateTime.TryParse(endDateTime, out DateTime end))
            {
                // Compare the DateTime objects
                // Returns true if End DateTime occurs after Start DateTime
                return start < end;
            }

            // Handle parsing errors if necessary
            return false;
        }
    }
}
