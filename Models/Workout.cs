namespace LocalLiftLog.Models
{
    public partial class Workout : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ObservableProperty]
        public string name;
        public int WorkoutTemplateId { get; set; }
        [ObservableProperty]
        public string date;
        public bool IsWorkoutLoaded { get; set; }
    }
}
