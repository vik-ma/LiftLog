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

        public static string GetCurrentFormattedTime(bool is24HourFormat)
        {
            // 24 Hour Clock Format
            if (is24HourFormat) return DateTime.Now.ToString("HH:mm:ss");

            // 12 Hour Clock Format
            return DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);
        }

        public static string FormatDateTimeToYmdString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string FormatDateTimeToTimestampString(DateTime dateTime, bool is24HourFormat)
        {
            // 24 Hour Clock Format
            if (is24HourFormat) return dateTime.ToString("HH:mm:ss");

            // 12 Hour Clock Format
            return dateTime.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);
        }

        public static string FormatDateTimeToYmdDateAndTimestampString(DateTime dateTime, bool is24HourFormat)
        {
            // 24 Hour Clock Format
            if (is24HourFormat) return dateTime.ToString("yyyy-MM-dd HH:mm:ss");

            // 12 Hour Clock Format
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
        }

        public static string FormatDateTimeStringToTimestamp(string dateTimeString, bool is24HourFormat)
        {
            if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
            {
                return FormatDateTimeToTimestampString(dateTime, is24HourFormat);
            }
            return "Invalid Date";
        }

        public static string FormatDateTimeStringToYmdDateAndTimestampString(string dateTimeString, bool is24HourFormat)
        {
            if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
            {
                return FormatDateTimeToYmdDateAndTimestampString(dateTime, is24HourFormat);
            }
            return "Invalid Date";
        }

        public static DateTime FormatDateTimeYmdStringToDateTime(string dateTimeString)
        {
            return DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime parsedDateTime)
                ? parsedDateTime     // Return DateTime object if parse was successful
                : DateTime.MinValue; // Fallback DateTime if invalid dateTimeString
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
