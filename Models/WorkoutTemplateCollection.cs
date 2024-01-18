namespace LocalLiftLog.Models
{
    public class WorkoutTemplateCollection
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Day { get; set; }
        public int WorkoutTemplateId { get; set; }
        public int WorkoutRoutineId { get; set; }
    }

}