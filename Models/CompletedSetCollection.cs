using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class CompletedSetCollection
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CompletedSetId { get; set; }
        public string DateCompleted { get; set; }
    }
}
