using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System.Collections.ObjectModel;

namespace LocalLiftLog.ViewModels
{
    public partial class ProgramListViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public ProgramListViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<Program> _programList = new();

        [ObservableProperty]
        private Program _operatingProgram = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        [ObservableProperty]
        private bool _isEditing = false;

        public async Task LoadProgramsAsync()
        {
            await ExecuteAsync(async () =>
            {
                ProgramList.Clear();

                var Programs = await _context.GetAllAsync<Program>();

                if (Programs is not null && Programs.Any())
                {
                    Programs ??= new ObservableCollection<Program>();

                    foreach (var Program in Programs)
                    {
                        ProgramList.Add(Program);
                    }
                }
            }, "Fetching Program List...");  
        }

        #nullable enable
        [RelayCommand]
        private void SetOperatingProgram(Program? Program)
        {
            OperatingProgram = Program ?? new();
            IsEditing = true;
        }

        [RelayCommand]
        private void CancelEditing()
        {
            IsEditing = false;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }


        [RelayCommand]
        private async Task SaveProgramAsync()
        {
            if (OperatingProgram is null)
                return;

            var (isValid, errorMessage) = OperatingProgram.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            OperatingProgram.UpdateDateTime();

            var busyText = OperatingProgram.Id == 0 ? "Creating Program..." : "Updating Program";

            await ExecuteAsync(async () =>
            {
                if (OperatingProgram.Id == 0)
                {
                    // Create Program
                    await _context.AddItemAsync<Program>(OperatingProgram);
                    ProgramList.Add(OperatingProgram);

                    int id = ProgramList[ProgramList.Count - 1].Id;

                    // Send user to ProgramDetailsPage of newly created Program
                    ViewProgramDetailsCommand.Execute(id);
                }
                else
                {
                    // Update Program
                    if (await _context.UpdateItemAsync<Program>(OperatingProgram))
                    {
                        var ProgramCopy = OperatingProgram.Clone();

                        var index = ProgramList.IndexOf(OperatingProgram);
                        ProgramList.RemoveAt(index);

                        ProgramList.Insert(index, ProgramCopy);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when updating Program.", "OK");
                        return;
                    }
                }
                #nullable disable
                SetOperatingProgramCommand.Execute(new());
            }, busyText);
        }

        [RelayCommand]
        private async Task DeleteProgramAsync(int id)
        {
            var Program = ProgramList.FirstOrDefault(p => p.Id == id);

            if (Program is null) {
                await Shell.Current.DisplayAlert("Error", "Program does not exist.", "OK");
                return;
            };

            bool userClickedDelete = await Shell.Current.DisplayAlert("Delete Program", $"Are you sure you want to delete {Program.Name}?", "Delete", "Cancel");

            if (!userClickedDelete) return;

            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<Program>(id))
                {
                    ProgramList.Remove(Program);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Program was not deleted.", "OK");
                }
            }, "Deleting Program...");
        }

        #nullable enable
        private async Task ExecuteAsync(Func<Task> operation, string? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText ?? "Processing...";
            try
            {
                #nullable disable
                await operation?.Invoke();
            }
            catch
            {

            }
            finally
            {
                IsBusy = false;
                BusyText = "Finished";
                IsEditing = false;
            }
        }

        [RelayCommand]
        private async Task ViewProgramDetails(int id)
        {
            var Program = ProgramList.FirstOrDefault(p => p.Id == id);

            if (Program is null)
            {
                await Shell.Current.DisplayAlert("Error", "Program does not exist", "OK");
                return;
            }
                
            var navigationParameter = new Dictionary<string, object>
            {
                ["Program"] = Program
            };

            IsEditing = false;
            OperatingProgram = Program;

            await Shell.Current.GoToAsync($"{nameof(ProgramDetailsPage)}?Id={id}", navigationParameter);
        }
    }
}
