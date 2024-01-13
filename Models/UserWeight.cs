namespace LocalLiftLog.Models
{
    public class UserWeight
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int BodyWeight { get; set; }
        public string DateTime { get; set; }
    }
}
