namespace LocalLiftLog.Pages;
public partial class WorkoutPage : ContentPage
{
    private readonly WorkoutViewModel _viewModel;

    private readonly ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

    private readonly Color InactiveTabDefaultBackgroundColor;
    private readonly Color InactiveTabHoverBackgroundColor;
    private readonly Color InactiveTabDefaultTextColor;
    private readonly Color InactiveTabHoverTextColor;

    public WorkoutPage(WorkoutViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InactiveTabDefaultBackgroundColor = ColorResource["VeryLightGray"] as Color;
        InactiveTabHoverBackgroundColor = ColorResource["MediumLightGray"] as Color;
        InactiveTabDefaultTextColor = ColorResource["VeryDarkGray"] as Color;
        InactiveTabHoverTextColor = ColorResource["Black"] as Color;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        // Prevent reloading old page if navigated
        // from WorkoutOperatingPage or ExerciseDetailsPage
        if (!_viewModel.IsWorkoutLoaded) 
        {
            _viewModel.LoadSelectedDateTime();
            await _viewModel.LoadSetList();
        }
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        DateTime selectedDate = e.NewDate;

        await _viewModel.UpdateWorkoutDate(selectedDate);
    }

    private void DropGestureRecognizer_DragOver(object sender, DragEventArgs e)
    {
        e.PlatformArgs.DragEventArgs.DragUIOverride.IsGlyphVisible = false;
        e.PlatformArgs.DragEventArgs.DragUIOverride.IsCaptionVisible = false;
    }

    private void DropGestureRecognizer_DragLeave(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.None;
    }

    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            Label label = frame.Content as Label;
            frame.BackgroundColor = InactiveTabHoverBackgroundColor;
            label.TextColor = InactiveTabHoverTextColor;
        }
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            Label label = frame.Content as Label;
            frame.BackgroundColor = InactiveTabDefaultBackgroundColor;
            label.TextColor = InactiveTabDefaultTextColor;
        }
    }
}