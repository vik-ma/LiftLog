namespace LocalLiftLog.Pages;

public partial class ExerciseDetailsPage : ContentPage
{
	private readonly ExerciseDetailsViewModel _viewModel;

    private readonly ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

    private readonly Color InactiveTabDefaultBackgroundColor;
    private readonly Color InactiveTabHoverBackgroundColor;
    private readonly Color InactiveTabDefaultTextColor;
    private readonly Color InactiveTabHoverTextColor;

    public ExerciseDetailsPage(ExerciseDetailsViewModel viewModel)
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
        await _viewModel.LoadSetsFromExerciseIdAsync();
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