using LocalLiftLog.Views;

namespace LocalLiftLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RoutineListPage), typeof(RoutineListPage));
            Routing.RegisterRoute(nameof(EditRoutineListPage), typeof(EditRoutineListPage));
            Routing.RegisterRoute(nameof(AddRoutinePage), typeof(AddRoutinePage));
            Routing.RegisterRoute(nameof(EditRoutinePage), typeof(EditRoutinePage));
        }
    }
}