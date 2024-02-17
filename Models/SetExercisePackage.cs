namespace LocalLiftLog.Models
{
    public partial class SetExercisePackage : ObservableObject
    {
        public Exercise Exercise { get; set; }
        public Set Set { get; set; }

        [ObservableProperty]
        public bool isBeingDragged;
        [ObservableProperty]
        public bool isBeingDraggedOver;
    }
}
