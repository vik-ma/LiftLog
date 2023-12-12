using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class SetWorkoutTemplatePackage
    {
        public WorkoutTemplate WorkoutTemplate { get; set; }
        public SetTemplate SetTemplate { get; set; }
        public bool IsEditing { get; set; }
    }
}
