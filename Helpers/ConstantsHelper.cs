namespace LocalLiftLog.Helpers
{
    public static class ConstantsHelper
    {
        public static readonly int SetTemplateDefaultMinValue = 0;
        public static readonly int SetTemplateDefaultMaxValue = 999;
        public static readonly int SetTemplateDefaultInputMinValue = 1;

        public static readonly int PercentInputMinValue = 1;
        public static readonly int PercentInputMaxValue = 100;

        public static readonly int CompletedSetMinValue = 0;
        public static readonly int CompletedSetMaxValue = 999;

        public static readonly int BodyWeightMinValue = 0;
        public static readonly int BodyWeightMaxValue = 999;
        public static readonly int BodyWeightInputMinValue = 0;

        public static readonly HashSet<string> ValidWeightUnits = new() { "kg", "lbs" };
        public static readonly HashSet<string> ValidDistanceUnits = new() { "km", "m", "mi", "ft", "yd" };

        public static readonly string DefaultWeightUnitMetricTrue = "kg";
        public static readonly string DefaultWeightUnitMetricFalse = "lbs";

        public static readonly string DefaultDistanceUnitMetricTrue = "km";
        public static readonly string DefaultDistanceUnitMetricFalse = "mi";

        public static readonly int ExerciseGroupMinValue = 0;
        public static readonly int ExerciseGroupMaxValue = ExerciseGroupDictionary.ExerciseGroupMaxValue;

    }
}
