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
    }
}
