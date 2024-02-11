namespace LocalLiftLog.Data
{
    public static class DefaultExercises
    {
        public static List<Exercise> DefaultExerciseList { get; } =
        [
            new() { Name="Bench Press", ExerciseGroupSetString="0,1" },
            new() { Name="Hammer Curl", ExerciseGroupSetString="2,3" },
            new() { Name="Lateral Raise", ExerciseGroupSetString="4" },
            new() { Name="Deadlift", ExerciseGroupSetString="5,6,7,8,9,10,11,17" },
            new() { Name="Calf Raise", ExerciseGroupSetString="12,18" },
            new() { Name="Sit-Ups", ExerciseGroupSetString="13" },
            new() { Name="Crush Gripper", ExerciseGroupSetString="14" },
            new() { Name="Running", ExerciseGroupSetString="15" },
            new() { Name="Other", ExerciseGroupSetString="16" },
        ];
    }
}
