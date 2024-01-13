namespace LocalLiftLog.Models
{
    public class SetWorkoutTemplatePackage
    {
        public WorkoutTemplate WorkoutTemplate { get; set; }
        public SetTemplate SetTemplate { get; set; }
        public bool IsEditing { get; set; }
    }
}
