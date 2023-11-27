using LocalLiftLog.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Data
{
    public class ExerciseDataManager
    {
        private List<Exercise> ExerciseList;

        private readonly DatabaseContext _context;
        public ExerciseDataManager(DatabaseContext context) 
        {
            InitializeExerciseList();
            _context = context;
        }

        private async void InitializeExerciseList()
        {
            ExerciseList = new List<Exercise>
            {
                new Exercise { Name="Bench Press", ExerciseGroupSet=new HashSet<int>(new[] { 0, 1 }) },
                new Exercise { Name="Hammer Curl", ExerciseGroupSet=new HashSet<int>(new[] { 2, 11 }) },
                new Exercise { Name="Lateral Raise", ExerciseGroupSet=new HashSet<int>(new[] { 3 }) },
                new Exercise { Name="Deadlift", ExerciseGroupSet=new HashSet<int>(new[] { 4, 5, 6, 7, 8, 9, 12 }) },
                new Exercise { Name="Calf Raise", ExerciseGroupSet=new HashSet<int>(new[] { 10 }) },
                new Exercise { Name="Crush Gripper", ExerciseGroupSet=new HashSet<int>(new[] { 13 }) },
                new Exercise { Name="Running", ExerciseGroupSet=new HashSet<int>(new[] { 14 }) },
                new Exercise { Name="Other", ExerciseGroupSet=new HashSet<int>(new[] { 15 }) },
            };

            var customExerciseList = await LoadCustomExercises();

            if (customExerciseList is null || !customExerciseList.Any()) return;

            var convertedCustomExerciseList = ConvertCustomExerciseListToExerciseList(customExerciseList);
        }

        private async Task<IEnumerable<CustomExercise>> LoadCustomExercises()
        {
            try
            {
                var customExerciseList = await _context.GetAllAsync<CustomExercise>();
                return customExerciseList;
            }
            catch
            {
                return null;
            }
        }

        private static List<Exercise> ConvertCustomExerciseListToExerciseList(IEnumerable<CustomExercise> customExerciseList)
        {
            var convertedExerciseList = new List<Exercise>();

            foreach (var customExercise in customExerciseList)
            {
                var customExerciseGroupSet = new HashSet<int>();

                Type type = customExercise.GetType();

                // Get properties that start with "IsExerciseGroup"
                var groupProperties = type.GetProperties()
                                          .Where(prop => prop.PropertyType == typeof(bool) && prop.Name.StartsWith("IsExerciseGroup"))
                                          .ToList();

                foreach (var property in groupProperties)
                {
                    // Extract the number from the property name
                    if (int.TryParse(property.Name.Substring("IsExerciseGroup".Length), out int groupNumber))
                    {
                        bool propertyValue = (bool)property.GetValue(customExercise);

                        if (propertyValue && groupNumber >= 0 && groupNumber <= 15)
                        {
                            customExerciseGroupSet.Add(groupNumber);
                        }
                    }
                }

                var convertedExercise = new Exercise { Name = customExercise.Name, ExerciseGroupSet=customExerciseGroupSet };
                
                convertedExerciseList.Add(convertedExercise);
            }

            return convertedExerciseList;
        }

        public IEnumerable<Exercise> GetExerciseList()
        {
            return ExerciseList;
        }

        public IEnumerable<Exercise> FilterExerciseListByExerciseGroup(int group)
        {
            if (group < 0 || group > 15) return null;

            return ExerciseList.Where(item => item.ExerciseGroupSet.Contains(group));
        }
    }
}
