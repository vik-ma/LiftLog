namespace LocalLiftLog.Models
{
    public class WorkoutTemplateListPopupPageHandler
    {
        public string ViewModelType { get; set; }
        public WorkoutViewModel WorkoutViewModel { get; set; }
        public ScheduleViewModel ScheduleViewModel { get; set; }
        public WorkoutDetailsViewModel WorkoutDetailsViewModel { get; set; }

        public dynamic GetViewModel()
        {
            return ViewModelType switch
            {
                "Workout" => WorkoutViewModel,
                "Schedule" => ScheduleViewModel,
                "WorkoutDetails" => WorkoutDetailsViewModel,
                _ => default,
            };
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var selectedWorkoutTemplate = (WorkoutTemplate)args.SelectedItem;

                if (ViewModelType == "Workout" && WorkoutViewModel is not null)
                {
                    await WorkoutViewModel.UpdateOperatingWorkoutTemplate(selectedWorkoutTemplate.Id);
                    WorkoutViewModel.ClosePopup();
                }
                if (ViewModelType == "Schedule" && ScheduleViewModel is not null)
                {
                    await ScheduleViewModel.AddWorkoutTemplateCollectionToDay(selectedWorkoutTemplate);
                    ScheduleViewModel.ClosePopup();
                }
                if (ViewModelType == "WorkoutDetails" && WorkoutDetailsViewModel is not null)
                {
                    await WorkoutDetailsViewModel.CopyWorkout(selectedWorkoutTemplate.Id);
                    WorkoutDetailsViewModel.ClosePopup();
                }
            }
        }

        public void OnFilterTextChanged(object sender, TextChangedEventArgs args)
        {
            string filterText = args.NewTextValue.ToLowerInvariant();

            if (ViewModelType == "Workout" && WorkoutViewModel is not null)
            {
                WorkoutViewModel.FilteredWorkoutTemplateList.Clear();

                if (string.IsNullOrWhiteSpace(filterText))
                    WorkoutViewModel.FilteredWorkoutTemplateList = new(WorkoutViewModel.WorkoutTemplateList);
                else
                    WorkoutViewModel.FilteredWorkoutTemplateList = new(WorkoutViewModel.WorkoutTemplateList.Where(item => item.Name != null && item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
            }
            if (ViewModelType == "Schedule" && ScheduleViewModel is not null)
            {
                ScheduleViewModel.FilteredWorkoutTemplateList.Clear();

                if (string.IsNullOrWhiteSpace(filterText))
                    ScheduleViewModel.FilteredWorkoutTemplateList = new(ScheduleViewModel.WorkoutTemplateList);
                else
                    ScheduleViewModel.FilteredWorkoutTemplateList = new(ScheduleViewModel.WorkoutTemplateList.Where(item => item.Name != null && item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
            }
            if (ViewModelType == "WorkoutDetails" && WorkoutDetailsViewModel is not null)
            {
                WorkoutDetailsViewModel.FilteredWorkoutTemplateList.Clear();

                if (string.IsNullOrWhiteSpace(filterText))
                    WorkoutDetailsViewModel.FilteredWorkoutTemplateList = new(WorkoutDetailsViewModel.WorkoutTemplateList);
                else
                    WorkoutDetailsViewModel.FilteredWorkoutTemplateList = new(WorkoutDetailsViewModel.WorkoutTemplateList.Where(item => item.Name != null && item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
            }
        }
    }
}
