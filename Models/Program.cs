using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Helpers;

namespace LocalLiftLog.Models
{
    public class Program
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastUpdateDateTime { get; set; }  
        public string Note { get; set; }
        public int ScheduleFactoryId { get; set; }
        
        public Program Clone() => MemberwiseClone() as Program;

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
