namespace LocalLiftLog.Pages;

public partial class CreateExercisePopupPage : Popup
{
	private readonly ExerciseListViewModel _viewModel;
	public CreateExercisePopupPage(ExerciseListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}