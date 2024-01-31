namespace LocalLiftLog.Models
{
    public class UserWeight
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double BodyWeight { get; set; }
        public string DateTime { get; set; }
        public string WeightUnit { get; set; }

        #nullable enable
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (BodyWeight < ConstantsHelper.BodyWeightInputMinValue || BodyWeight > ConstantsHelper.BodyWeightMaxValue) return (false, "Invalid Weight Number");

            if (!ConstantsHelper.ValidWeightUnits.Contains(WeightUnit)) return (false, "Invalid Weight Unit");

            return (true, null);
        }
    }
}
