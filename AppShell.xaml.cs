using LocalLiftLog.Pages;

namespace LocalLiftLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RoutineDetailsPage), typeof(RoutineDetailsPage));
        }
    }
}