using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalLiftLog.Models
{
    public class WeeklySchedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Day1TemplateId { get; set; } = "1";
        public string Day2TemplateId { get; set; } = "1,2,3";
        public string Day3TemplateId { get; set; }
        public string Day4TemplateId { get; set; }
        public string Day5TemplateId { get; set; } = "3";
        public string Day6TemplateId { get; set; }
        public string Day7TemplateId { get; set; }

        public WeeklySchedule Clone() => MemberwiseClone() as WeeklySchedule;

        public int[] GetDayTemplateIdIntArray(string day)
        {
            Dictionary<string, string> dayDictionary = new Dictionary<string, string>
            {
                { "Day1", "Day1TemplateId" },
                { "Day2", "Day2TemplateId" },
                { "Day3", "Day3TemplateId" },
                { "Day4", "Day4TemplateId" },
                { "Day5", "Day5TemplateId" },
                { "Day6", "Day6TemplateId" },
                { "Day7", "Day7TemplateId" }
            };

            if (!dayDictionary.ContainsKey(day)) return Array.Empty<int>();
                
            try
            {
                object dayIdString = this.GetType().GetProperty(dayDictionary[day]).GetValue(this, null);

                string[] stringIdArray = dayIdString.ToString().Split(',');

                int[] intIdArray = stringIdArray.Select(s => int.Parse(s)).ToArray();

                return intIdArray;
            }
            catch
            {
                return Array.Empty<int>();
            }
        }

        public bool ValidateTemplateIdInput(string input)
        {
            // Pattern to check if string is empty, just one integer, or a list of integers separated by commas
            string regExPattern = @"^(\d+,)*\d*$";

            return Regex.IsMatch(input, regExPattern);
        }
    }
}
