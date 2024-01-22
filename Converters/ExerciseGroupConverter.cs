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

            if (value is List<int> exerciseGroupIntList)
            {
                List<string> exerciseGroupStringList = new();

                foreach (var group in exerciseGroupIntList)
                {
                    if (exerciseGroupDict.TryGetValue(group, out string exercise))
                    {
                        exerciseGroupStringList.Add(exercise);
                    } 
                    else
                    {
                        exerciseGroupStringList.Add("Invalid Group");
                    }
                }

                return exerciseGroupStringList;
            }

            if (value is string exerciseGroupString)
            {
                if (string.IsNullOrEmpty(exerciseGroupString)) return "None";

                List<string> exerciseGroupStringList = new();

                string[] groupStrings = exerciseGroupString.Split(",");

                foreach (var group in groupStrings)
                {
                    if (int.TryParse(group.Trim(), out int exerciseGroupInt))
                    {
                        if (exerciseGroupDict.TryGetValue(exerciseGroupInt, out string exercise))
                        {
                            exerciseGroupStringList.Add(exercise);
                        }
                        else return "Invalid group";
                    }
                }

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
