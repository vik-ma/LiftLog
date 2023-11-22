using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class CompletedSet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
        public string DateCompleted { get; set; }
        public int CompletedWorkoutId { get; set; }
        public int Weight { get; set; }
    }
}
