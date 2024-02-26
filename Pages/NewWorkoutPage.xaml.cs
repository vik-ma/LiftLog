namespace LocalLiftLog.Pages;

public partial class NewWorkoutPage : ContentPage
{
    private readonly NewWorkoutViewModel _viewModel;
    public NewWorkoutPage(NewWorkoutViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}