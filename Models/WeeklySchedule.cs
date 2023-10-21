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
        public string Day1TemplateId { get; set; } = "aa";
        public string Day2TemplateId { get; set; }
        public string Day3TemplateId { get; set; }
        public string Day4TemplateId { get; set; }
        public string Day5TemplateId { get; set; } = "asd";
        public string Day6TemplateId { get; set; }
        public string Day7TemplateId { get; set; }

        public WeeklySchedule Clone() => MemberwiseClone() as WeeklySchedule;
    }
}
