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
        public int Weight { get; set; }

        public int CompletedSetCollectionId { get; set; }
        
    }
}
