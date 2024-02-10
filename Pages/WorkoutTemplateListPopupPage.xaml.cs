namespace LocalLiftLog.Pages;

public partial class WorkoutTemplateListPopupPage : Popup
{
    private readonly WorkoutTemplateListPopupPageHandler _handler;
    public WorkoutTemplateListPopupPage(WorkoutTemplateListPopupPageHandler handler)
	{
		InitializeComponent();
        _handler = handler;
        BindingContext = _handler.GetViewModel();
    }

    private void OnWorkoutTemplateItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        _handler.OnItemSelected(sender, args);
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs args)
    {
        _handler.OnFilterTextChanged(sender, args);
    }
}