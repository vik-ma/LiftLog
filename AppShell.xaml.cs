using LocalLiftLog.Pages;

namespace LocalLiftLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(WorkoutRoutineListPage), typeof(WorkoutRoutineListPage));
            Routing.RegisterRoute(nameof(WorkoutRoutineDetailsPage), typeof(WorkoutRoutineDetailsPage));
            Routing.RegisterRoute(nameof(ScheduleListPage), typeof(ScheduleListPage));
            Routing.RegisterRoute(nameof(WeeklySchedulePage), typeof(WeeklySchedulePage));
            Routing.RegisterRoute(nameof(CustomSchedulePage), typeof(CustomSchedulePage));
            Routing.RegisterRoute(nameof(WorkoutTemplateListPage), typeof(WorkoutTemplateListPage));
            Routing.RegisterRoute(nameof(WorkoutDetailsPage), typeof(WorkoutDetailsPage));
            Routing.RegisterRoute(nameof(SetTemplateListPage), typeof(SetTemplateListPage));
            Routing.RegisterRoute(nameof(CompletedSetListPage), typeof(CompletedSetListPage));
            Routing.RegisterRoute(nameof(CompletedWorkoutListPage), typeof(CompletedWorkoutListPage));
            Routing.RegisterRoute(nameof(TimePeriodListPage), typeof(TimePeriodListPage));
            Routing.RegisterRoute(nameof(CustomExerciseListPage), typeof(CustomExerciseListPage));
            Routing.RegisterRoute(nameof(ExerciseListPage), typeof(ExerciseListPage));
            Routing.RegisterRoute(nameof(StartedWorkoutPage), typeof(StartedWorkoutPage));
            Routing.RegisterRoute(nameof(CreateSetTemplatePage), typeof(CreateSetTemplatePage));
        }
    }
}