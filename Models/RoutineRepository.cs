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
            new Routine { RoutineId = 1, Name="Routine 1", OrderSlot=1},
            new Routine { RoutineId = 2, Name="Routine 2", OrderSlot=2},
            new Routine { RoutineId = 3, Name="Routine 3", OrderSlot=3},
            new Routine { RoutineId = 4, Name="Routine 4", OrderSlot=4},
        };

        public static List<Routine> GetRoutines() => _routines;

        public static Routine GetRoutineById(int routineId)
        {
            var routine = _routines.FirstOrDefault(x => x.RoutineId == routineId);

            if (routine != null) 
            {
                return new Routine
                {
                    RoutineId = routineId,
                    Name = routine.Name,
                    OrderSlot = routine.OrderSlot,
                };
            }
            return null;
        }

        public static void UpdateRoutine(int routineId, Routine routine)
        {
            if (routineId != routine.RoutineId) return;

            var routineToUpdate = _routines.FirstOrDefault(x => x.RoutineId == routineId);

            if (routine != null)
            {
                routineToUpdate.Name = routine.Name;
                routineToUpdate.OrderSlot = routine.OrderSlot;
            }
        }
    }
}
