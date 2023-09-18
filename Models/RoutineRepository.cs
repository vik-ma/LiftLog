using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public static class RoutineRepository
    {
        public static List<Routine> _routines = new List<Routine>()
        {
            new Routine { Name="Routine 1", OrderSlot=1},
            new Routine { Name="Routine 2", OrderSlot=2},
            new Routine { Name="Routine 3", OrderSlot=3},
            new Routine { Name="Routine 4", OrderSlot=4},
        };

        public static List<Routine> GetRoutines() => _routines;

        public static Routine GetRoutineById(int routineId)
        {
            return _routines.FirstOrDefault(x => x.RoutineId == routineId);
        }
    }
}
