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
            Routing.RegisterRoute(nameof(RoutineSchedulePage), typeof(RoutineSchedulePage));
        }
    }
}