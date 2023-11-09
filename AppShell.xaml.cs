using LocalLiftLog.Pages;

namespace LocalLiftLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RoutineListPage), typeof(RoutineListPage));
            Routing.RegisterRoute(nameof(RoutineDetailsPage), typeof(RoutineDetailsPage));
            Routing.RegisterRoute(nameof(ScheduleListPage), typeof(ScheduleListPage));
            Routing.RegisterRoute(nameof(WeeklySchedulePage), typeof(WeeklySchedulePage));
            Routing.RegisterRoute(nameof(CustomSchedulePage), typeof(CustomSchedulePage));
            Routing.RegisterRoute(nameof(WorkoutTemplateListPage), typeof(WorkoutTemplateListPage));
        }
    }
}