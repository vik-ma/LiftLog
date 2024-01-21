namespace LocalLiftLog.Data
{
    public class ExerciseDataManager
    {
        private readonly DatabaseContext _context;

        private readonly List<Exercise> ExerciseList = new();
        public ExerciseDataManager(DatabaseContext context) 
        {
            _context = context;

            LoadExerciseList();
        }

        #nullable enable
        private async Task ExecuteAsync(Func<Task> operation)
        {
            try
            {
                #nullable disable
                await operation?.Invoke();
            }
            catch
            {

            }
            finally
            {

            }
        }

        private async Task CreateDefaultExerciseList()
        {
            List<Exercise> defaultExerciseList = new()
            {
                new() { Name="Bench Press", ExerciseGroupSetString="0,1" },
                new() { Name="Hammer Curl", ExerciseGroupSetString="2,3" },
                new() { Name="Lateral Raise", ExerciseGroupSetString="4" },
                new() { Name="Deadlift", ExerciseGroupSetString="5,6,7,8,9,10,11" },
                new() { Name="Calf Raise", ExerciseGroupSetString="12" },
                new() { Name="Sit-Ups", ExerciseGroupSetString="13" },
                new() { Name="Crush Gripper", ExerciseGroupSetString="14" },
                new() { Name="Running", ExerciseGroupSetString="15" },
                new() { Name="Other", ExerciseGroupSetString="16" },
            };

            foreach (var exercise in defaultExerciseList)
            {
                await CreateExerciseAsync(exercise);
            }
        }

        private async Task CreateExerciseAsync(Exercise exercise)
        {
            await ExecuteAsync(async () =>
            {
                await _context.AddItemAsync<Exercise>(exercise);
                ExerciseList.Add(exercise);
            });
        }

        private async void LoadExerciseList()
        {
            await LoadExercisesAsync();

            if (!ExerciseList.Any())
            {
                // Add default Exercises if ExerciseList is empty
                await CreateDefaultExerciseList();
            }
        }

        private async Task LoadExercisesAsync()
        {
            await ExecuteAsync(async () =>
            {
                ExerciseList.Clear();

                var exercises = await _context.GetAllAsync<Exercise>();

                if (exercises is not null && exercises.Any())
                {
                    exercises ??= new ObservableCollection<Exercise>();

                    foreach (var exercise in exercises)
                    {
                        ExerciseList.Add(exercise);
                    }
                }
            });
        }

        //private async Task<IEnumerable<CustomExercise>> LoadCustomExerciseFromDatabase()
        //{
        //    try
        //    {
        //        var customExerciseList = await _context.GetAllAsync<CustomExercise>();
        //        return customExerciseList;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //private static List<Exercise> ConvertCustomExerciseListToExerciseList(IEnumerable<CustomExercise> customExerciseList)
        //{
        //    var convertedExerciseList = new List<Exercise>();

        //    //foreach (var customExercise in customExerciseList)
        //    //{
        //    //    var customExerciseGroupSet = new HashSet<int>();

        //    //    Type type = customExercise.GetType();

        //    //    // Get properties that start with "IsExerciseGroup"
        //    //    var groupProperties = type.GetProperties()
        //    //                              .Where(prop => prop.PropertyType == typeof(bool) && prop.Name.StartsWith("IsExerciseGroup"))
        //    //                              .ToList();

        //    //    foreach (var property in groupProperties)
        //    //    {
        //    //        // Extract the number from the property name
        //    //        if (int.TryParse(property.Name.AsSpan("IsExerciseGroup".Length), out int groupNumber))
        //    //        {
        //    //            bool propertyValue = (bool)property.GetValue(customExercise);

        //    //            if (propertyValue && groupNumber >= 0 && groupNumber <= 15)
        //    //            {
        //    //                customExerciseGroupSet.Add(groupNumber);
        //    //            }
        //    //        }
        //    //    }

        //    //    var convertedExercise = new Exercise { Name = customExercise.Name, ExerciseGroupSet=customExerciseGroupSet };

        //    //    convertedExerciseList.Add(convertedExercise);
        //    //}

        //    return convertedExerciseList;
        //}

        //public async Task UpdateFullExerciseList()
        //{
        //    await LoadCustomExerciseList();

        //    FullExerciseList = ExerciseList.Concat(CustomExerciseList ?? Enumerable.Empty<Exercise>());
        //}

        //public async Task<IEnumerable<Exercise>> GetFullExerciseList()
        //{
        //    await UpdateFullExerciseList();

        //    return FullExerciseList;
        //}

        //public IEnumerable<Exercise> FilterExerciseListByExerciseGroups(HashSet<int> groupSet)
        //{
        //    return null;
        //    //if (groupSet is null || groupSet.Count == 0) return FullExerciseList;

        //    //return ExerciseList.Where(item => groupSet.Any(group => item.ExerciseGroupSet.Contains(group)));
        //}

        //public async Task<bool> ValidateUniqueExerciseName(string exerciseName)
        //{
        //    await UpdateFullExerciseList();

        //    var exerciseNameExists = FullExerciseList.FirstOrDefault(x => x.Name == exerciseName);

        //    if (exerciseNameExists is null) return false;

        //    return true;
        //} 
    }
}
