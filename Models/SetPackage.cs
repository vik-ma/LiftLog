namespace LocalLiftLog.Models
{
    public partial class SetPackage : ObservableObject
    {
        public SetTemplate SetTemplate { get; set; }
        public CompletedSet CompletedSet { get; set; }

        [ObservableProperty]
        public bool isEditingSetProperties;
    }
}
