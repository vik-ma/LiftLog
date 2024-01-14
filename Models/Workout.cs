namespace LocalLiftLog.Models
{
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkoutTemplateId { get; set; }
        public string Date { get; set; }
    }
}
