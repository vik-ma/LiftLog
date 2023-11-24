using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class TimePeriod
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public bool IsPeriodOngoing { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; } 
    }
}
