using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Helpers;

namespace LocalLiftLog.Models
{
    public class RoutineList
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastUpdateDateTime { get; set; }  
        public string Focus { get; set; }

        public string Test { get; set; }    

        public RoutineList Clone() => MemberwiseClone() as RoutineList;

        #nullable enable
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, $"{nameof(Name)} is required.");
            }
            return (true, null);
        }
        public void UpdateDateTime()
        {
            LastUpdateDateTime = DateTimeHelper.GetCurrentFormattedDateTime();
        }
    }
}
