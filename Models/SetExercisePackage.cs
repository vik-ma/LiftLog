namespace LocalLiftLog.Models
{
    public class SetExercisePackage
    {
        public Exercise Exercise { get; set; }
        public Set Set { get; set; }
        public bool IsBeingDragged { get; set; }
        public bool IsBeingDraggedOver { get; set; }
    }
}
