namespace LocalLiftLog.Pages;
public partial class ExerciseListPage : ContentPage
{
    private readonly ExerciseListViewModel _viewModel;
    public ExerciseListPage(ExerciseListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadExerciseGroupIntList();
        _viewModel.LoadExerciseList();
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedExercise = (int)args.SelectedItem;
            _viewModel.AddExerciseGroupToFilterList(selectedExercise);
        }
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs args)
    {
        string filterText = args.NewTextValue.ToLowerInvariant();

        _viewModel.FilteredExerciseList.Clear();
        _viewModel.ResetFilterList();

        if (string.IsNullOrWhiteSpace(filterText))
            _viewModel.FilteredExerciseList = new(_viewModel.ExerciseList);
        else
            _viewModel.FilteredExerciseList = new(_viewModel.ExerciseList.Where(item => item.Name != null && item.Name.Contains(filterText, StringComparison.InvariantCultureIgnoreCase)));
    }
}