using System;
using System.Collections.Generic;
using System.Globalization;
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
            string ymdDateString = DateTime.Now.ToString("yyyy-MM-dd");
            return ymdDateString;
        }

        public static string GetCurrentFormattedTime()
        {
            string timeString = DateTime.Now.ToString("HH:mm:ss");
            return timeString;
        }

        public static string FormatDateTimeToYmdString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
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
            string dateFormat = "yyyy-MM-dd";

            // Parse the DateTime strings into DateTime objects
            if (DateTime.TryParseExact(startYmdDate, dateFormat, null, DateTimeStyles.None, out DateTime start) &&
                DateTime.TryParseExact(endYmdDate, dateFormat, null, DateTimeStyles.None, out DateTime end))
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

        public static string GetWeekdayOfYmdDateString(string ymdDateString)
        {
            if (DateTime.TryParseExact(ymdDateString, "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.DayOfWeek.ToString();
            }

            return "Invalid Date";
        }
    }
}
