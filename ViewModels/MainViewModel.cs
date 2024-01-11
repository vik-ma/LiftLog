using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.ViewModels;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Models;

namespace LocalLiftLog.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private UserPreferencesViewModel userSettingsViewModel;

        public MainViewModel(DatabaseContext context, UserPreferencesViewModel userSettings)
        {
            _context = context;
            userSettingsViewModel = userSettings;
        }

        public async Task LoadUserPreferences()
        {
            await UserSettingsViewModel.LoadUserPreferencesAsync();
        }

        [RelayCommand]
        private async Task GoToWorkoutRoutineList()
        {
            await Shell.Current.GoToAsync($"{nameof(WorkoutRoutineListPage)}");
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

        [RelayCommand]
        private async Task GoToSelectWorkout()
        {
            await Shell.Current.GoToAsync($"{nameof(SelectWorkoutPage)}");
        }

        [RelayCommand]
        private async Task GoToUserPreferences()
        {
            await Shell.Current.GoToAsync($"{nameof(UserPreferencesPage)}");
        }

        [RelayCommand]
        private async Task GoToUserWeight()
        {
            await Shell.Current.GoToAsync($"{nameof(UserWeightPage)}");
        }
    }
}
