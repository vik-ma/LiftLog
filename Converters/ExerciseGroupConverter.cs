using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Converters
{
    public class ExerciseGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exerciseGroupDict = new Dictionary<int, string>
            {
                { 0, "Chest" },
                { 1, "Triceps" },
                { 2, "Biceps" },
                { 3, "Shoulders" },
                { 4, "Upper Back" },
                { 5, "Mid Back (Lats)" },
                { 6, "Lower Back" },
                { 7, "Quadriceps" },
                { 8, "Glutes" },
                { 9, "Hamstrings" },
                { 10, "Calves" },
                { 11, "Forearms" },
                { 12, "Core (Abs)" },
                { 13, "Grip" },
                { 14, "Cardio" },
                { 15, "Other" },
            };

            List<string> exerciseGroupStringList = new();

            if (value is HashSet<int> exerciseGroups)
            {
                foreach (var group in exerciseGroups) 
                {
                    if (exerciseGroupDict.ContainsKey(group))
                    {
                        exerciseGroupStringList.Add(exerciseGroupDict[group]);
                    }
                    else return "Invalid Group";
                }

                if (!exerciseGroupStringList.Any()) return "No Exercise Group Added";

                var exerciseGroupsString = String.Join(", ", exerciseGroupStringList);

                return exerciseGroupsString;
            }

            return "Invalid Group";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
