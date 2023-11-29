using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class CustomExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsExerciseGroup0 { get; set; }
        public bool IsExerciseGroup1 { get; set; }
        public bool IsExerciseGroup2 { get; set; }
        public bool IsExerciseGroup3 { get; set; }
        public bool IsExerciseGroup4 { get; set; }
        public bool IsExerciseGroup5 { get; set; }
        public bool IsExerciseGroup6 { get; set; }
        public bool IsExerciseGroup7 { get; set; }
        public bool IsExerciseGroup8 { get; set; }
        public bool IsExerciseGroup9 { get; set; }
        public bool IsExerciseGroup10 { get; set; }
        public bool IsExerciseGroup11 { get; set; }
        public bool IsExerciseGroup12 { get; set; }
        public bool IsExerciseGroup13 { get; set; }
        public bool IsExerciseGroup14 { get; set; }
        public bool IsExerciseGroup15 { get; set; }

        public bool ValidateExerciseGroups()
        {
            // Check if at least one Exercise Group has been added
            // Returns true if at least one IsExerciseGroup bool is set to true, otherwise false

            Type type = this.GetType();

            // Get properties that start with "IsExerciseGroup"
            var groupProperties = type.GetProperties()
                                      .Where(prop => prop.PropertyType == typeof(bool) && prop.Name.StartsWith("IsExerciseGroup"))
                                      .ToList();

            foreach (var property in groupProperties)
            {
                if ((bool)property.GetValue(this))
                {
                    // If at least one IsExerciseGroup property is set to true
                    return true;
                }
            }
            // If no IsExerciseGroup properties are set to true
            return false;
        }
    }
}
