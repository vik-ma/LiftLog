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
        private async Task GoToRoutineList()
        {
            await Shell.Current.GoToAsync($"{nameof(RoutineListPage)}");
        }
    }
}
