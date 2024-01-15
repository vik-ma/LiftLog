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

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadExerciseGroupIntList();
        await _viewModel.LoadExercisesAsync();
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedExercise = (int)args.SelectedItem;
            _viewModel.AddExerciseGroupToFilterList(selectedExercise);
        }
    }

    void OnFilterTextChanged(object sender, TextChangedEventArgs args)
    {
        string filterText = args.NewTextValue.ToLowerInvariant();

        _viewModel.FilteredExerciseList.Clear();

        if (string.IsNullOrWhiteSpace(filterText))
        {
            _viewModel.FilteredExerciseList = new(_viewModel.ExerciseList);
        }
        else
        {
            // Filter the list based on the user input
            var filteredItems = _viewModel.ExerciseList.Where(item => item.Name.ToLowerInvariant().Contains(filterText));

            foreach (var item in filteredItems)
            {
                _viewModel.FilteredExerciseList.Add(item);
            }
        }
    }
}