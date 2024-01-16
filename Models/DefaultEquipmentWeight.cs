namespace LocalLiftLog.Models
{
    public partial class DefaultEquipmentWeight : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public double weight;
        [ObservableProperty]
        public string weightUnit;

        #nullable enable
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) return (false, "Name can't be empty.");

            if (Weight == 0) return (false, "Weight can't be 0.");

            if (Weight < 0) return (false, "Weight can't be negative.");

            return (true, null);
        }
    }
}
