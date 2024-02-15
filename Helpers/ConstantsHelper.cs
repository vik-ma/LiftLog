namespace LocalLiftLog.Helpers
{
    public static class ConstantsHelper
    {
        public static readonly int SetTemplateDefaultMinValue = 0;
        public static readonly int SetTemplateDefaultMaxValue = 999;
        public static readonly int SetTemplateDefaultInputMinValue = 1;

        public static readonly int SetTrackingMinValue = 0;
        public static readonly int SetTrackingMaxValue = 999;

        public static readonly int BodyWeightMinValue = 0;
        public static readonly int BodyWeightMaxValue = 999;

        public static readonly HashSet<string> ValidWeightUnits = ["kg", "lbs"];
        public static readonly HashSet<string> ValidDistanceUnits = ["km", "m", "mi", "ft", "yd"];

        public static readonly string DefaultWeightUnitMetric = "kg";
        public static readonly string DefaultWeightUnitImperial = "lbs";

        public static readonly string DefaultDistanceUnitMetric = "km";
        public static readonly string DefaultDistanceUnitImperial = "mi";

        public static readonly int DefaultBarbellWeightMetric = 20;
        public static readonly int DefaultDumbbellWeightMetric = 2;

        public static readonly int DefaultBarbellWeightImperial = 45;
        public static readonly int DefaultDumbbellWeightImperial = 5;

        public static readonly int ExerciseGroupMinValue = 0;
        public static readonly int ExerciseGroupMaxValue = ExerciseGroupDictionary.ExerciseGroupMaxValue;

    }
}
