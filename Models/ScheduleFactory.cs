using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class ScheduleFactory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsScheduleWeekly { get; set; } = true;
        public int ScheduleId { get; set; }

        public ScheduleFactory Clone() => MemberwiseClone() as ScheduleFactory;
    }
}
