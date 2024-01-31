namespace LocalLiftLog.Models
{
    public class SetWorkoutTemplatePackage
    {
        public WorkoutTemplate WorkoutTemplate { get; set; }
        public Set Set { get; set; }
        public bool IsEditing { get; set; }
    }
}
