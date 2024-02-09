namespace LocalLiftLog.Pages;
public partial class CreateSetTemplatePage : ContentPage
{
    private readonly CreateSetTemplateViewModel _viewModel;
    public CreateSetTemplatePage(CreateSetTemplateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadExerciseList();
        _viewModel.InitializeSetWorkoutTemplatePackage();
        _viewModel.SetDefaultUnitValues();
    }

    private void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            var selectedExercise = (Exercise)args.SelectedItem;
            _viewModel.SelectedExercise = selectedExercise;
        }
    }
    private void OnWeightUnitPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            _viewModel.SelectedWeightUnit = (string)picker.SelectedItem;
        }
    }

    private void OnDistanceUnitPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            _viewModel.SelectedDistanceUnit = (string)picker.SelectedItem;
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