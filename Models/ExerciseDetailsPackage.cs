namespace LocalLiftLog.Models
{
    public class ExerciseDetailsPackage
    {
        public Exercise Exercise { get; set; }
        public bool IsComingFromWorkoutPage { get; set; }
        public Workout Workout { get; set; }
    }
}
