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
            string dateTimeString = DateTime.Now.ToString();
            return dateTimeString;
        }
        public static string GetCurrentFormattedYmdDate()
        {
            string ymdDateString = DateTime.Now.ToString("yyyyMMdd");
            return ymdDateString;
        }

        public static string FormatDateTimeToYmdString(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
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

        public static bool ValidateStartAndEndYmdDate(string startYmdDate, string endYmdDate)
        {
            // Define the expected date format
            string dateFormat = "yyyyMMdd";

            // Parse the DateTime strings into DateTime objects
            if (DateTime.TryParseExact(startYmdDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime start) &&
                DateTime.TryParseExact(endYmdDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime end))
            {
                // Compare the DateTime objects
                // Returns true if End DateTime occurs after Start DateTime
                return start < end;
            }

            // Handle parsing errors if necessary
            return false;
        }

        public static string GetWeekdayOfDate(DateTime date)
        {
            return date.DayOfWeek.ToString();
        }
    }
}
