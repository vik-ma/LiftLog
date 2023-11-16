using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class SetTemplateCollection
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SetTemplateId { get; set; }
        public int SetListIndex { get; set; }
    }
}
