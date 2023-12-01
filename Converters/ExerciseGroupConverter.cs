using LocalLiftLog.Data;
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
            var exerciseGroupDict = ExerciseGroupDictionary.ExerciseGroupDict;

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
