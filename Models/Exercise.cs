namespace LocalLiftLog.Models
{
    public class Exercise
    {
        public string Name { get; set; }
        public HashSet<int> ExerciseGroupSet { get; set; }
        public string ExerciseGroupSetString { get; set; }

        public HashSet<int> GetExerciseGroupHashSet()
        {
            HashSet<int> exerciseGroupHashSet = new();

            string[] groupStrings = ExerciseGroupSetString.Split(',');

            foreach (var group in groupStrings)
            {
                if (int.TryParse(group.Trim(), out int groupInt))
                {
                    if (groupInt >= ConstantsHelper.ExerciseGroupMinValue && groupInt <= ConstantsHelper.ExerciseGroupMaxValue)
                    {
                        exerciseGroupHashSet.Add(groupInt);
                    }
                }
            }

            return exerciseGroupHashSet;
        }

        public void SetExerciseGroupSetString(HashSet<int> exerciseGroupHashSet)
        {
            // Do not update empty HashSets
            if (exerciseGroupHashSet is null || !exerciseGroupHashSet.Any()) return;

            // Ensure that all HashSet values are within bounds
            if (!exerciseGroupHashSet.All(x => x >= ConstantsHelper.ExerciseGroupMinValue && x <= ConstantsHelper.ExerciseGroupMaxValue)) return;

            // Create string of HashSet with comma separated values
            string hashSetString = string.Join(",", exerciseGroupHashSet);

            ExerciseGroupSetString = hashSetString;
        }
    }
}
