namespace LocalLiftLog.Models
{
    public class CustomSchedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ScheduleFactoryId { get; set; }
        public int WorkoutRoutineId { get; set; }
        public int NumDaysInSchedule { get; set; }
    }
}
