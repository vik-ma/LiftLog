using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutTemplate), nameof(WorkoutTemplate))]
    public partial class WorkoutDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private WorkoutTemplateViewModel _workoutTemplateViewModel;

        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        public WorkoutDetailsViewModel(WorkoutTemplateViewModel workoutTemplateViewModel, DatabaseContext context)
        {
            _workoutTemplateViewModel = workoutTemplateViewModel;
            _context = context;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
