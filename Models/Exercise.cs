using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class Exercise
    {
        public string Name { get; set; }
        public HashSet<int> ExerciseGroupSet { get; set; }
    }
}
