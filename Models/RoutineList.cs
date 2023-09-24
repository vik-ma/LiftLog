using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class RoutineList
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderSlot { get; set; }

        public RoutineList Clone() => MemberwiseClone() as RoutineList;

        #nullable enable
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, $"{nameof(Name)} is required.");
            }
            else if (OrderSlot <= 0)
            {
                return (false, $"{nameof(OrderSlot)} should be grater than 0.");
            }
            return (true, null);
        }
    }
}
