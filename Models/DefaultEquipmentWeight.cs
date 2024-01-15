namespace LocalLiftLog.Models
{
    public partial class DefaultEquipmentWeight : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public int weight;
        [ObservableProperty]
        public string weightUnit;
    }
}
