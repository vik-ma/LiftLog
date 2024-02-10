namespace LocalLiftLog.Models
{
    public class WorkoutTemplate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SetListOrder { get; set; }
        public string Note { get; set; }

        #nullable enable
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrEmpty(Name)) return (false, "Workout Name is empty!");

            return (true, null);
        }
    }
}
