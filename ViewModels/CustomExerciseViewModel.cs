using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq.Expressions;

namespace LocalLiftLog.ViewModels
{
    public partial class CustomExerciseViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public CustomExerciseViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
