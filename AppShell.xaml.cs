using LocalLiftLog.Pages;

namespace LocalLiftLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ProgramListPage), typeof(ProgramListPage));
            Routing.RegisterRoute(nameof(ProgramDetailsPage), typeof(ProgramDetailsPage));
            Routing.RegisterRoute(nameof(ScheduleListPage), typeof(ScheduleListPage));
            Routing.RegisterRoute(nameof(WeeklySchedulePage), typeof(WeeklySchedulePage));
            Routing.RegisterRoute(nameof(CustomSchedulePage), typeof(CustomSchedulePage));
            Routing.RegisterRoute(nameof(WorkoutTemplateListPage), typeof(WorkoutTemplateListPage));
            Routing.RegisterRoute(nameof(SetTemplateListPage), typeof(SetTemplateListPage));
        }
    }
}