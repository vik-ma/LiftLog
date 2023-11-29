using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public MainViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [RelayCommand]
        private async Task GoToProgramList()
        {
            await Shell.Current.GoToAsync($"{nameof(ProgramListPage)}");
        }

        [RelayCommand]
        private async Task GoToScheduleList()
        {
            await Shell.Current.GoToAsync($"{nameof(ScheduleListPage)}");
        }

        [RelayCommand]
        private async Task GoToWorkoutTemplate()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutTemplateListPage)}");
        }

        [RelayCommand]
        private async Task GoToSetTemplate()
        {
            await Shell.Current.GoToAsync($"{nameof(SetTemplateListPage)}");
        }

        [RelayCommand]
        private async Task GoToCompletedSet()
        {
            await Shell.Current.GoToAsync($"{nameof(CompletedSetListPage)}");
        }

        [RelayCommand]
        private async Task GoToCompletedWorkout()
        {
            await Shell.Current.GoToAsync($"{nameof(CompletedWorkoutListPage)}");
        }

        [RelayCommand]
        private async Task GoToTimePeriod()
        {
            await Shell.Current.GoToAsync($"{nameof(TimePeriodListPage)}");
        }

        [RelayCommand]
        private async Task GoToCustomExercise()
        {
            await Shell.Current.GoToAsync($"{nameof(CustomExerciseListPage)}");
        }

        [RelayCommand]
        private async Task GoToExerciseList()
        {
            await Shell.Current.GoToAsync($"{nameof(ExerciseListPage)}");
        }
    }
}
