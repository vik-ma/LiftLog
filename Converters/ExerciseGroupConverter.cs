namespace LocalLiftLog.Converters
{
    public class ExerciseGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exerciseGroupDict = ExerciseGroupDictionary.ExerciseGroupDict;

            if (value is HashSet<int> exerciseGroups)
            {
                List<string> exerciseGroupStringList = new();

                foreach (var group in exerciseGroups) 
                {
                    if (exerciseGroupDict.TryGetValue(group, out string exercise))
                    {
                        exerciseGroupStringList.Add(exercise);
                    }
                    else return "Invalid Group";
                }

                if (!exerciseGroupStringList.Any()) return "None";

                var exerciseGroupsString = String.Join(", ", exerciseGroupStringList);

                return exerciseGroupsString;
            }

            if (value is int exerciseGroup)
            {
                if (exerciseGroupDict.TryGetValue(exerciseGroup, out string exercise))
                {
                    return exercise;
                }
            }

            return "Invalid Group";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
