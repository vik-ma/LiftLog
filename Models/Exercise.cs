namespace LocalLiftLog.Models
{
    public class Exercise
    {
        public string Name { get; set; }
        public HashSet<int> ExerciseGroupSet { get; set; }
    }
}
