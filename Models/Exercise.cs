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

            string[] groupStrings = ExerciseGroupSetString.Split(',');

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
            // Do not update empty HashSets
            if (exerciseGroupHashSet is null || !exerciseGroupHashSet.Any()) return;

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
    }
}
