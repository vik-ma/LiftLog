namespace LocalLiftLog.Models
{
    public class Exercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExerciseGroupSetString { get; set; }

        public HashSet<int> GetExerciseGroupHashSet()
        {
            HashSet<int> exerciseGroupHashSet = new();

            // Return empty HashSet if ExerciseGroupSetString is not set
            if (string.IsNullOrEmpty(ExerciseGroupSetString)) return exerciseGroupHashSet;

            string[] groupStrings = ExerciseGroupSetString.Split(",");

            foreach (var group in groupStrings)
            {
                if (int.TryParse(group.Trim(), out int exerciseGroupInt))
                {
                    if (IsExerciseGroupIntValid(exerciseGroupInt))
                    {
                        exerciseGroupHashSet.Add(exerciseGroupInt);
                    }
                }
            }

            return exerciseGroupHashSet;
        }

        public void SetExerciseGroupSetString(HashSet<int> exerciseGroupHashSet)
        {
            if (exerciseGroupHashSet is null) return;

            // If HashSet is empty
            if (!exerciseGroupHashSet.Any()) 
            {
                ExerciseGroupSetString = null;
                return; 
            }

            if (!IsHashSetValid(exerciseGroupHashSet)) return;

            // Create string of HashSet with comma separated values
            string hashSetString = string.Join(",", exerciseGroupHashSet);

            ExerciseGroupSetString = hashSetString;
        }

        public static bool IsHashSetValid(HashSet<int> exerciseGroupHashSet)
        {
            // Returns true if all HashSet values are within the bounds of valid Exercise Groups
            return exerciseGroupHashSet.All(x => x >= ConstantsHelper.ExerciseGroupMinValue && x <= ConstantsHelper.ExerciseGroupMaxValue);
        }

        public static bool IsExerciseGroupIntValid(int exerciseGroupInt)
        {
            // Returns true if int is within the bounds of valid Exercise Groups
            return exerciseGroupInt >= ConstantsHelper.ExerciseGroupMinValue && exerciseGroupInt <= ConstantsHelper.ExerciseGroupMaxValue;
        }

        public bool AddExerciseGroup(int exerciseGroupInt)
        {
            // Returns true if value was successfully changed, false if not

            if (!IsExerciseGroupIntValid(exerciseGroupInt)) return false;

            HashSet<int> exerciseGroupHashSet = GetExerciseGroupHashSet();

            if (exerciseGroupHashSet.Contains(exerciseGroupInt)) return false;

            exerciseGroupHashSet.Add(exerciseGroupInt);

            SetExerciseGroupSetString(exerciseGroupHashSet);

            return true;
        }

        public bool RemoveExerciseGroup(int exerciseGroupInt)
        {
            // Returns true if value was successfully changed, false if not

            if (!IsExerciseGroupIntValid(exerciseGroupInt)) return false;

            HashSet<int> exerciseGroupHashSet = GetExerciseGroupHashSet();

            if (!exerciseGroupHashSet.Contains(exerciseGroupInt)) return false; 

            exerciseGroupHashSet.Remove(exerciseGroupInt);

            SetExerciseGroupSetString(exerciseGroupHashSet);

            return true;
        }

        #nullable enable
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) return (false, "Name can't be empty!");

            if (string.IsNullOrEmpty(ExerciseGroupSetString)) return (false, "At least one Exercise Group must be added!");

            if (IsHashSetValid(GetExerciseGroupHashSet())) return (false, "Invalid Exercise Group!");

            return (true, null);
        }
    }
}
