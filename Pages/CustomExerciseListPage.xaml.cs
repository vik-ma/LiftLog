namespace LocalLiftLog.Pages;
public partial class CustomExerciseListPage : ContentPage
{
	private readonly CustomExerciseViewModel _viewModel;
	public CustomExerciseListPage(CustomExerciseViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadExerciseList();
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedExercise = (Exercise)args.SelectedItem;
            await _viewModel.RestoreExercise(selectedExercise);
        }
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs args)
    {
        string filterText = args.NewTextValue.ToLowerInvariant();

        _viewModel.FilteredDefaultExerciseList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
            _viewModel.FilteredDefaultExerciseList = new(_viewModel.DefaultExerciseList);
        else
            _viewModel.FilteredDefaultExerciseList = new(_viewModel.DefaultExerciseList.Where(item => item.Name != null && item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
    }
}