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
            Routing.RegisterRoute(nameof(WeeklyScheduleListPage), typeof(WeeklyScheduleListPage));
            Routing.RegisterRoute(nameof(WeeklySchedulePage), typeof(WeeklySchedulePage));
            Routing.RegisterRoute(nameof(WorkoutTemplateListPage), typeof(WorkoutTemplateListPage));
        }
    }
}