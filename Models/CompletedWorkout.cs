using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class CompletedWorkout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CompletedSetCollectionId { get; set; }
        public bool IsCompleted { get; set; }
        public string DateCompleted { get; set; }
    }
}
