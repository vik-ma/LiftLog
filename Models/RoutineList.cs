using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class RoutineList
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderSlot { get; set; }

        public RoutineList Clone() => MemberwiseClone() as RoutineList;
    }
}
