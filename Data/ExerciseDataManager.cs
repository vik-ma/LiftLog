using LocalLiftLog.Models;
using SQLite;
using System;
using System.Collections;
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
        private List<Exercise> CustomExerciseList;

        public readonly Dictionary<int, string> ExerciseGroupDict;

        private readonly DatabaseContext _context;
        public ExerciseDataManager(DatabaseContext context) 
        {
            InitializeExerciseList();
            _context = context;

            ExerciseGroupDict = new Dictionary<int, string>
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
        }

        private void InitializeExerciseList()
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
        }

        private async Task LoadCustomExerciseList()
        {
            CustomExerciseList?.Clear();

            var customExerciseDatabaseList = await LoadCustomExerciseFromDatabase();

            if (customExerciseDatabaseList is null || !customExerciseDatabaseList.Any()) return;

            CustomExerciseList = ConvertCustomExerciseListToExerciseList(customExerciseDatabaseList);
        }

        private async Task<IEnumerable<CustomExercise>> LoadCustomExerciseFromDatabase()
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
                    if (int.TryParse(property.Name.AsSpan("IsExerciseGroup".Length), out int groupNumber))
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

        public async Task<IEnumerable<Exercise>> GetFullExerciseList()
        {
            await LoadCustomExerciseList();

            IEnumerable<Exercise> combinedExerciseList = ExerciseList.Concat(CustomExerciseList ?? Enumerable.Empty<Exercise>());

            return combinedExerciseList;
        }

        public IEnumerable<Exercise> FilterExerciseListByExerciseGroup(int group)
        {
            if (group < 0 || group > 15) return null;

            return ExerciseList.Where(item => item.ExerciseGroupSet.Contains(group));
        }

        public async Task<bool> ValidateUniqueExerciseName(string exerciseName)
        {
            var exerciseList = await GetFullExerciseList();

            var exerciseNameExists = exerciseList.FirstOrDefault(x => x.Name == exerciseName);

            if (exerciseNameExists is null) return false;

            return true;
        } 
    }
}
