namespace LocalLiftLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(WorkoutRoutineListPage), typeof(WorkoutRoutineListPage));
            Routing.RegisterRoute(nameof(WorkoutRoutineDetailsPage), typeof(WorkoutRoutineDetailsPage));
            Routing.RegisterRoute(nameof(WorkoutTemplateListPage), typeof(WorkoutTemplateListPage));
            Routing.RegisterRoute(nameof(WorkoutDetailsPage), typeof(WorkoutDetailsPage));
            Routing.RegisterRoute(nameof(CompletedSetListPage), typeof(CompletedSetListPage));
            Routing.RegisterRoute(nameof(CompletedWorkoutListPage), typeof(CompletedWorkoutListPage));
            Routing.RegisterRoute(nameof(TimePeriodListPage), typeof(TimePeriodListPage));
            Routing.RegisterRoute(nameof(CustomExerciseListPage), typeof(CustomExerciseListPage));
            Routing.RegisterRoute(nameof(ExerciseListPage), typeof(ExerciseListPage));
            Routing.RegisterRoute(nameof(StartedWorkoutPage), typeof(StartedWorkoutPage));
            Routing.RegisterRoute(nameof(CreateSetTemplatePage), typeof(CreateSetTemplatePage));
            Routing.RegisterRoute(nameof(SelectWorkoutPage), typeof(SelectWorkoutPage));
            Routing.RegisterRoute(nameof(UserPreferencesPage), typeof(UserPreferencesPage));
            Routing.RegisterRoute(nameof(UserWeightPage), typeof(UserWeightPage));
            Routing.RegisterRoute(nameof(NewWorkoutPage), typeof(NewWorkoutPage));
            Routing.RegisterRoute(nameof(WorkoutPage), typeof(WorkoutPage));
            Routing.RegisterRoute(nameof(WorkoutListPage), typeof(WorkoutListPage));
            Routing.RegisterRoute(nameof(SchedulePage), typeof(SchedulePage));
        }
    }
}