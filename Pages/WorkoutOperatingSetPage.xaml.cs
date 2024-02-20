namespace LocalLiftLog.Pages;

public partial class WorkoutOperatingSetPage : ContentPage
{
    private readonly WorkoutViewModel _viewModel;

    private readonly ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

    private readonly Color InactiveTabDefaultBackgroundColor;
    private readonly Color InactiveTabHoverBackgroundColor;
    private readonly Color InactiveTabDefaultTextColor;
    private readonly Color InactiveTabHoverTextColor;
    public WorkoutOperatingSetPage(WorkoutViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InactiveTabDefaultBackgroundColor = ColorResource["VeryLightGray"] as Color;
        InactiveTabHoverBackgroundColor = ColorResource["MediumLightGray"] as Color;
        InactiveTabDefaultTextColor = ColorResource["VeryDarkGray"] as Color;
        InactiveTabHoverTextColor = ColorResource["Black"] as Color;
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