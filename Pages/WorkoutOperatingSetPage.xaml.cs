namespace LocalLiftLog.Pages;

public partial class WorkoutOperatingSetPage : ContentPage
{
    private readonly WorkoutViewModel _viewModel;

    private readonly ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

    private readonly Color InactiveTabDefaultBackgroundColor;
    private readonly Color InactiveTabHoverBackgroundColor;
    private readonly Color InactiveTabDefaultTextColor;
    private readonly Color InactiveTabHoverTextColor;
    private readonly Color SetDefaultBackgroundColor;
    private readonly Color SetHoverBackgroundColor;
    public WorkoutOperatingSetPage(WorkoutViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InactiveTabDefaultBackgroundColor = ColorResource["VeryLightGray"] as Color;
        InactiveTabHoverBackgroundColor = ColorResource["MediumLightGray"] as Color;
        InactiveTabDefaultTextColor = ColorResource["VeryDarkGray"] as Color;
        InactiveTabHoverTextColor = ColorResource["Black"] as Color;
        SetDefaultBackgroundColor = ColorResource["White"] as Color;
        SetHoverBackgroundColor = ColorResource["VeryLightGray"] as Color;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(600), () =>
        {
            // Scroll to OperatingSetExercisePackage 600 ms after page load
            SetListCollectionView.ScrollTo(_viewModel.OperatingSetExercisePackage);
            return false;
        });
    }

    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            Label label = frame.Content as Label;
            frame.BackgroundColor = InactiveTabHoverBackgroundColor;
            label.TextColor = InactiveTabHoverTextColor;
        }

        if (sender is Border border)
        {
            border.BackgroundColor = SetHoverBackgroundColor;
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

        if (sender is Border border)
        {
            border.BackgroundColor = SetDefaultBackgroundColor;
        }
    }
}