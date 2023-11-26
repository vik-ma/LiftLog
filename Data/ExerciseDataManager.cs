using LocalLiftLog.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Data
{
    public class ExerciseDataManager
    {
        public List<Exercise> ExerciseList { get; set; }
        public ExerciseDataManager() 
        {
            InitializeExerciseList();
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
    }
}
