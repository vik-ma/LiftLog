namespace LocalLiftLog.Models
{
    public partial class SetPackage : ObservableObject
    {
        public Set Set { get; set; }
        public CompletedSet CompletedSet { get; set; }

        [ObservableProperty]
        public bool isEditingSetProperties;
    }
}
