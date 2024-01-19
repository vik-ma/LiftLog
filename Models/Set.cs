namespace LocalLiftLog.Models
{
    public partial class Set : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ExerciseName { get; set; }
        [ObservableProperty]
        public string note;
        public bool IsWarmupSet { get; set; }
        [ObservableProperty]
        public string timeCompleted;
        public double Weight { get; set; }
        public int Reps { get; set; }
        public int Rir { get; set; }
        public int Rpe { get; set; }
        public int TimeInSeconds { get; set; }
        public double Distance { get; set; }
        public double CardioResistance { get; set; }
        public int PercentCompleted { get; set; }
    }
}
