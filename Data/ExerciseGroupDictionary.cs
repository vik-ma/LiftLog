namespace LocalLiftLog.Data
{
    public static class ExerciseGroupDictionary
    {
        public static Dictionary<int, string> ExerciseGroupDict { get; } = new()
        {
            { 0, "Chest" },
            { 1, "Triceps" },
            { 2, "Biceps" },
            { 3, "Forearms" },
            { 4, "Shoulders" },
            { 5, "Upper Back" },
            { 6, "Mid Back (Lats)" },
            { 7, "Lower Back" },
            { 8, "Glutes" },
            { 9, "Adductors" },
            { 10, "Quadriceps" },
            { 11, "Hamstrings" },
            { 12, "Calves" },
            { 13, "Core" },
            { 14, "Grip" },
            { 15, "Cardio" },
            { 16, "Other" },
            { 17, "Back" },
            { 18, "Legs" },
        };

        public static Dictionary<string, int> FlippedExerciseGroupDict { get; } = new()
        {
            { "Chest", 0 },
            { "Triceps", 1 },
            { "Biceps", 2 },
            { "Forearms", 3 },
            { "Shoulders", 4 },
            { "Upper Back", 5 },
            { "Mid Back (Lats)", 6 },
            { "Lower Back", 7 },
            { "Glutes", 8 },
            { "Adductors", 9 },
            { "Quadriceps", 10 },
            { "Hamstrings", 11 },
            { "Calves", 12 },
            { "Core", 13 },
            { "Grip", 14 },
            { "Cardio", 15 },
            { "Other", 16 },
            { "Back", 17 },
            { "Legs", 18 },
        };

        // Change the value of this property when adding/removing ExerciseGroupDict key(s) to the new max value
        public static int ExerciseGroupMaxValue { get; } = 18;

        public static List<int> GetSortedExerciseGroupList()
        {
            // Sorts ExerciseGroupDict by its values (alphabetically)
            // and returns a list of the keys in that order
            return ExerciseGroupDict.OrderBy(pair => pair.Value)
                                   .Select(pair => pair.Key)
                                   .ToList();
        }
    }
}
