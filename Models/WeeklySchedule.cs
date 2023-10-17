using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class WeeklySchedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Day1TemplateId { get; set; } = 2;
        public int Day2TemplateId { get; set; }
        public int Day3TemplateId { get; set; }
        public int Day4TemplateId { get; set; }
        public int Day5TemplateId { get; set; } = 5;
        public int Day6TemplateId { get; set; }
        public int Day7TemplateId { get; set; }
    }
}
