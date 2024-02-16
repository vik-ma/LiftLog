namespace LocalLiftLog.Models
{
    public class SetGroup : List<Set>
    {
        public string Date { get; private set; }

        public SetGroup(string date, List<Set> sets) : 
            base(sets) 
        {
            Date = date;
        }
    }
}
